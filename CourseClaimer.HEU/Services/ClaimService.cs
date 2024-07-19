using System.Collections.Generic;
using System.Net;
using CourseClaimer.HEU.Shared.Dto;
using CourseClaimer.HEU.Shared.Enums;
using CourseClaimer.HEU.Shared.Extensions;
using CourseClaimer.HEU.Shared.Models.Database;
using CourseClaimer.HEU.Shared.Models.JWXK;
using CourseClaimer.HEU.Shared.Models.JWXK.Roots;
using CourseClaimer.HEU.Shared.Models.Runtime;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;

namespace CourseClaimer.HEU.Services
{
    public class ClaimService(
        ILogger<ClaimService> logger,
        AuthorizeService authorizeService,
        IServiceProvider serviceProvider,
        ICapPublisher capBus)
    {
        public async Task MakeUserFinished(Entity entity)
        {
            var dbContext = serviceProvider.GetRequiredService<ClaimDbContext>();
            var customer = await dbContext.Customers.FirstAsync(c => c.UserName == entity.username);
            customer.IsFinished = true;
            await dbContext.SaveChangesAsync();
        }

        public async Task LogClaimRecord(Entity entity, Row @class, bool success)
        {
            var dbContext = serviceProvider.GetRequiredService<ClaimDbContext>();
            dbContext.ClaimRecords.Add(new ClaimRecord()
            {
                IsSuccess = success,
                UserName = entity.username,
                Course = @class.KCM + @class.XGXKLB,
            });
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Row>> GetAvailableList(Entity entity)
        {
            var res = await entity.GetRowList().ToResponseDto<ListRoot>();
            if (!res.IsSuccess) return [];
            var availableRows = res.Data.data.rows.Where(q => q.classCapacity > q.numberOfSelected);
            //if (availableRows.Count() != 0)
            //    logger.LogInformation($"AvailableList:{entity.username} found available course {string.Join('|', availableRows.Select(c => c.KCM))}");
            foreach (var row in availableRows)
            {
                await capBus.PublishAsync("ClaimService.RowAvailable", row);
                var secret = entity.Secrets.Find(s => s.KCH == row.KCH);
                if (secret.secretVal != row.secretVal)
                {
                    secret.secretVal = row.secretVal;
                    secret.classId = row.JXBID;
                }
            }
            return availableRows.ToList();
        }

        public async Task<List<Row>> GetAllList(Entity entity)
        {
            var res = await entity.GetRowList().ToResponseDto<ListRoot>();
            //logger.LogInformation($"AllList:{entity.username} found available course {string.Join('|', res.Data.data.rows.Select(c => c.KCM))}");
            //find all unadded rows in AllRows
            foreach (var row in res.Data.data.rows.Where(r => ProgramExtensions.AllRows.All(ar => ar.KCH != r.KCH)))
            {
                ProgramExtensions.AllRows.Add(new() { KCH = row.KCH, KCM = row.KCM, XGXKLB = row.XGXKLB });
                await capBus.PublishAsync("ClaimService.RowAdded", row);
            }
            entity.Secrets.AddRange(res.Data.data.rows.Select(row => new RowSecretDto
            {
                KCH = row.KCH,
                secretVal = row.secretVal,
                classId = row.JXBID
            }));
            return res.IsSuccess ? res.Data.data.rows.ToList() : [];
        }

        public async Task<AddResult> Add(Entity entity, Row @class)
        {
            var res = await entity.Add(@class).ToResponseDto<AddRoot>();
            if (res.InnerCode == HttpStatusCode.OK)
            {
                logger.LogInformation($"Add:{entity.username} has claimed {@class.KCM}({@class.XGXKLB})");
                entity.done.Add(@class);
                return AddResult.Success;
            }
            if (res.InnerMessage.Contains("请求过快")) return AddResult.OverSpeed;
            if (res.InnerMessage.Contains("已选满5门，不可再选") || res.InnerMessage.Contains("学分超过"))
                return AddResult.Full;
            if (res.InnerMessage.Contains("容量已满")) return AddResult.Failed;
            if (res.InnerMessage.Contains("选课结果中") || res.InnerMessage.Contains("不能重复选课") ||
                res.InnerMessage.Contains("冲突")) return AddResult.Conflict;
            if (res.InnerMessage.Contains("请重新登录")) return AddResult.AuthorizationExpired;
            logger.LogWarning($"Add:{entity.username} when claiming {@class.KCM}, server reported {res.InnerMessage}");
            var dbContext = serviceProvider.GetRequiredService<ClaimDbContext>();
            dbContext.EntityRecords.Add(new EntityRecord()
            {
                UserName = entity.username,
                Message = $"Add:{entity.username} when claiming {@class.KCM}, server reported {res.InnerMessage}"
            });
            await dbContext.SaveChangesAsync();
            return AddResult.UnknownError;
        }

        public async Task<ValidateResult> ValidateClaim(Entity entity, Row @class)
        {
            var res = await entity.ValidateClaim(@class).ToResponseDto<SelectRoot>();
            if (res.IsSuccess)
            {
                return res.Data.data.Any(q => q.KCH == @class.KCH) ? ValidateResult.Success : ValidateResult.Miss;
            }
            return ValidateResult.UnknownError;
        }

        public async Task Claim(Entity entity, Row @class)
        {
            entity.IsAddPending = true;
            while (true)
            {
                var res = await Add(entity, @class);
                switch (res)
                {
                    case AddResult.Success:
                        ValidateResult validate;
                        do validate = await ValidateClaim(entity, @class);
                        while(validate == ValidateResult.UnknownError);
                        await LogClaimRecord(entity, @class, validate == ValidateResult.Success);
                        entity.IsAddPending = false;
                        if (validate != ValidateResult.Success) return;
                        entity.done.Add(@class);
                        if (entity.done.Count < 5) return;
                        await MakeUserFinished(entity);
                        entity.finished = true;
                        return;
                    case AddResult.OverSpeed:
                        var dbContext = serviceProvider.GetRequiredService<ClaimDbContext>();
                        logger.LogWarning($"Claim:{entity.username} OverSpeed when claiming {@class.KCM}");
                        dbContext.EntityRecords.Add(new EntityRecord()
                        {
                            UserName = entity.username,
                            Message = $"Claim:{entity.username} OverSpeed when claiming {@class.KCM}"
                        });
                        await dbContext.SaveChangesAsync();
                        continue;
                    case AddResult.Full:
                        entity.IsAddPending = false;
                        entity.finished = true;
                        await MakeUserFinished(entity);
                        return;
                    case AddResult.Failed:
                        entity.IsAddPending = false;
                        logger.LogInformation($"Claim:{entity.username} Failed to claim {@class.KCM}");
                        await LogClaimRecord(entity, @class, false);
                        return;
                    case AddResult.Conflict:
                        entity.IsAddPending = false;
                        entity.SubscribedRows.Remove(@class.KCH);
                        return;
                    case AddResult.AuthorizationExpired:
                        entity.IsAddPending = true;
                        LoginResult loginResult;
                        do loginResult = await authorizeService.MakeUserLogin(entity);
                        while (loginResult == LoginResult.WrongCaptcha);
                        if (loginResult == LoginResult.WrongPassword) entity.finished = true;
                        entity.IsAddPending = false;
                        return;
                    case AddResult.UnknownError:
                        entity.IsAddPending = false;
                        return;
                }
                break;
            }
        }
    }
}
