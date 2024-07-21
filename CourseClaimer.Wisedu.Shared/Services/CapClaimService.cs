using CourseClaimer.Wisedu.Shared.Extensions;
using CourseClaimer.Wisedu.Shared.Models.JWXK;
using CourseClaimer.Wisedu.Shared.Models.Runtime;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

namespace CourseClaimer.Wisedu.Shared.Services
{
    public class CapClaimService(ILogger<CapClaimService> logger, ClaimService claimService) : ICapSubscribe
    {
        [CapSubscribe("ClaimService.RowAvailable")]
        public async Task CapClaimRow(Row row)
        {
            foreach (var entity in ProgramExtensions.Entities.Where(entity =>
                         entity.SubscribedRows.Contains(row.KCH) && !entity.IsAddPending))
            {
                logger.LogInformation($"CapClaimRow:{entity.username} Ready to claim {row.KCM}");
                _ = claimService.Claim(entity, row);
            }
        }

        [CapSubscribe("ClaimService.RowAdded")]
        public async Task CapAddRow(Row row)
        {
            foreach (var entity in ProgramExtensions.Entities.Where(entity =>
                         (entity.courses.Count == 0 || entity.courses.Any(c => row.KCM.Contains(c))) &&
                         (entity.category.Count == 0 || entity.category.Any(c => c == row.XGXKLB))))
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
