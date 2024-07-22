using CourseClaimer.Wisedu.Shared.Extensions;
using CourseClaimer.Wisedu.Shared.Models.JWXK;
using CourseClaimer.Wisedu.Shared.Models.Runtime;
using DotNetCore.CAP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CourseClaimer.Wisedu.Shared.Services
{
    public class CapClaimService(ILogger<CapClaimService> logger, ClaimService claimService,IConfiguration configuration) : ICapSubscribe
    {
        private int takeNum = Convert.ToInt32(configuration["CapTakeNum"]); 

        [CapSubscribe("ClaimService.RowAvailable")]
        public async Task CapClaimRow(Row row)
        {
            foreach (var entity in ProgramExtensions.Entities.Where(entity =>
                         entity.SubscribedRows.Contains(row.KCH) && !entity.IsAddPending)
                         .OrderByDescending(e => e.priority)
                         .Take(takeNum))
            {
                _ = claimService.Claim(entity, row);
                logger.LogInformation($"CapClaimRow:{entity.username} is claiming {row.KCM}");
            }
        }

        [CapSubscribe("ClaimService.RowAdded")]
        public async Task CapAddRow(Row row)
        {
            foreach (var entity in ProgramExtensions.Entities.Where(entity =>
                         (entity.courses.Count == 0 || entity.courses.Any(c => row.KCM.Contains(c))) &&
                         (entity.category.Count == 0 || entity.category.Any(c => c == row.XGXKLB)))
                         .OrderByDescending(e => e.priority))
            {
                entity.SubscribedRows.Add(row.KCH);
                logger.LogInformation($"CapAddRow:{entity.username} added course {row.KCM}");
            }
        }

        public async Task StartAsync(Entity entity, CancellationToken token = default)
        {
            entity.SubscribedRows.AddRange(ProgramExtensions.AllRows.Where(row =>
                    (entity.courses.Count == 0 || entity.courses.Any(c => row.KCM.Contains(c))) &&
                    (entity.category.Count == 0 || entity.category.Any(c => c == row.XGXKLB)))
                .Select(r => r.KCH));
            await claimService.GetAllList(entity);
            while (!token.IsCancellationRequested)
            {
                await claimService.GetAvailableList(entity);
            }
        }
    }
}
