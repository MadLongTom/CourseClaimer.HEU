using CourseClaimer.HEU.Shared.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace CourseClaimer.HEU.Services;

public class ClaimDbContext : DbContext
{

    public ClaimDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ClaimRecord> ClaimRecords { get; set; }
    public DbSet<EntityRecord> EntityRecords { get; set; }
}