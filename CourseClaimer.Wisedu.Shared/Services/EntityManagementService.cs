using System.Text.Json;
using CourseClaimer.Wisedu.Shared.Dto;
using CourseClaimer.Wisedu.Shared.Enums;
using CourseClaimer.Wisedu.Shared.Extensions;
using CourseClaimer.Wisedu.Shared.Models.Database;
using CourseClaimer.Wisedu.Shared.Models.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CourseClaimer.Wisedu.Shared.Services
{
    public class EntityManagementService(
        AuthorizeService authorizeService,
        CapClaimService capClaimService,
        ILogger<EntityManagementService> logger,
        ClaimDbContext dbContext) : IHostedService
    {
        public List<WorkInfo> WorkInfos { get; set; } = [];

        public async Task<(string,string)> ExportAllCustomer()
        {
            var customers = await dbContext.Customers.AsNoTracking().ToListAsync();
            foreach (var customer in customers)
            {
                customer.Id = Guid.Empty;
            }
            //save to file
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"customers-{DateTime.Now.ToFileTime()}.json");
            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(customers));
            return (JsonSerializer.Serialize(customers), path);
        }

        public async Task<string> ExportAllClaims()
        {
            var claims = await dbContext.ClaimRecords.AsNoTracking().Where(c => c.IsSuccess == true)
                .OrderBy(x => x.UserName).ThenBy(x => x.ClaimTime).ToListAsync();
            return JsonSerializer.Serialize(claims);
        }

        public async Task AddCustomersFromJson(string json)
        {
            var customers = JsonSerializer.Deserialize<List<Customer>>(json);
            //remove exists customers
            var exists = await dbContext.Customers.Select(c => c.UserName).ToListAsync();
            customers.RemoveAll(c => exists.Contains(c.UserName));
            await dbContext.Customers.AddRangeAsync(customers);
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> AddCustomer(Customer customer)
        {
            //return if user already exists
            if (await dbContext.Customers.AnyAsync(c => c.UserName == customer.UserName))
            {
                return false;
            }
            await dbContext.Customers.AddAsync(customer);
            await dbContext.SaveChangesAsync();
            await RefreshCustomerStatus(customer);
            return true;
        }

        public async Task DeleteCustomer(string userName)
        {
            var customer = await dbContext.Customers.FirstAsync(c => c.UserName == userName);
            customer.IsFinished = true;
            await RefreshCustomerStatus(customer);
            dbContext.Customers.Remove(customer);
            await dbContext.SaveChangesAsync();
        }

        public async Task <QueryDto<RowDto>> QueryRow(int page, int pageSize)
        {
            return new QueryDto<RowDto>
            {
                Total = ProgramExtensions.AllRows.Count,
                Data = ProgramExtensions.AllRows.Skip((page - 1) * pageSize).Take(pageSize).ToList()
            };
        }

        public async Task<QueryDto<Customer>> QueryUser(int page,int pageSize)
        {
            var query = dbContext.Customers.AsQueryable();
            var total = await query.CountAsync();
            var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new QueryDto<Customer>
            {
                Total = total,
                Data = data
            };
        }

        public async Task<QueryDto<JobRecord>> QueryJob(int page, int pageSize)
        {
            var query = dbContext.JobRecords.AsQueryable();
            var total = await query.CountAsync();
            var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new QueryDto<JobRecord>
            {
                Total = total,
                Data = data
            };
        }

        public async Task<QueryDto<ClaimRecord>> QueryRecord(int page, int pageSize)
        {
            var query = dbContext.ClaimRecords.AsQueryable();
            var total = await query.CountAsync();
            var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new QueryDto<ClaimRecord>
            {
                Total = total,
                Data = data
            };
        }

        public async Task<QueryDto<EntityRecord>> QueryEntity(int page, int pageSize)
        {
            var query = dbContext.EntityRecords.AsQueryable();
            var total = await query.CountAsync();
            var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new QueryDto<EntityRecord>
            {
                Total = total,
                Data = data
            };
        }

        public async Task EditCustomer(Customer customer)
        {
            var local = dbContext.Set<Customer>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(customer.Id));

            // Check if local is not null 
            if (local != null)
            {
                // Detach
                dbContext.Entry(local).State = EntityState.Detached;
            }
            // Set Modified state
            dbContext.Entry(customer).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();
            await RefreshCustomerStatus(customer);
        }

        static readonly Dictionary<string, string> xgxklbs = new()
        {
            { "A", "19人文素质与文化传承（A）" }, { "B", "19艺术鉴赏与审美体验（B）" }, { "C", "19社会发展与公民责任（C）" }, { "D", "19自然科学与工程技术（D）" },
            { "E", "19三海一核与国防建设（E）" }, { "F", "19创新思维与创业实践（F）" }, { "A0", "19中华传统文化类（A0）" }
        };

        public async Task RefreshCustomerStatus(Customer customer)
        {
            var workinfo = WorkInfos.FirstOrDefault(w => w.Entity.username == customer.UserName);
            if (!customer.IsFinished)
            {
                if (workinfo == null)
                {
                    var entity = new Entity(customer.UserName, customer.Password,
                        customer.Categories == string.Empty
                            ? []
                            : customer.Categories.Split(',').Select(p => xgxklbs[p]).ToList(),
                        customer.Course == string.Empty ? [] : [.. customer.Course.Split(',')], [], false, null,customer.Priority);
                    ProgramExtensions.Entities.Add(entity);
                    var cts = new CancellationTokenSource();
                    LoginResult loginResult;
                    do
                    {
                        loginResult = await authorizeService.MakeUserLogin(entity);
                    } while (loginResult == LoginResult.WrongCaptcha);

                    if (loginResult == LoginResult.WrongPassword)
                    {
                        logger.LogError($"Login:{entity.username}: Wrong Password");
                        dbContext.EntityRecords.Add(new EntityRecord()
                        {
                            UserName = customer.UserName,
                            Message = "Login: Wrong Password"
                        });
                        dbContext.Customers.Remove(customer);
                        await dbContext.SaveChangesAsync();
                        return;
                    }

                    var task = capClaimService.StartAsync(entity,cts.Token);

                    WorkInfos.Add(new WorkInfo
                    {
                        Entity = entity,
                        task = task,
                        CancellationTokenSource = cts
                    });
                }
            }
            else
            {
                if (workinfo != null)
                {
                    await workinfo.CancellationTokenSource.CancelAsync();
                    ProgramExtensions.Entities.Remove(workinfo.Entity);
                    WorkInfos.Remove(workinfo);
                }
            }
        }

        public async Task WebStartAsync(CancellationToken cancellationToken = default)
        {
            var customers = await dbContext.Customers.AsNoTracking().OrderByDescending(c => c.Priority).ToListAsync(cancellationToken);
            foreach (var customer in customers)
            {
                customer.IsFinished = false;
                await dbContext.SaveChangesAsync(cancellationToken);
                await RefreshCustomerStatus(customer);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            var customers = await dbContext.Customers.AsNoTracking().ToListAsync(cancellationToken);
            foreach (var customer in customers)
            {
                customer.IsFinished = true;
                await dbContext.SaveChangesAsync(cancellationToken);
                await RefreshCustomerStatus(customer);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            foreach (var workInfo in WorkInfos)
            {
                await workInfo.CancellationTokenSource.CancelAsync();
            }
            WorkInfos.Clear();
            ProgramExtensions.Entities.Clear();
        }
    }
}
