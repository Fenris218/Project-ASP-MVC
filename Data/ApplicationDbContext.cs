using Microsoft.EntityFrameworkCore;
using QuanLyPhongNet.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }



    public DbSet<User> Users { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<UsageRecord> UsageRecords { get; set; }
}
