using APP.Models.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Database;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RememberUser> RememberUser => Set<RememberUser>();
    public DbSet<ErrorMaster> ErrorMaster => Set<ErrorMaster>();
    public DbSet<PLCSetting> PLCSetting => Set<PLCSetting>();
    public DbSet<PrinterSetting> PrinterSetting => Set<PrinterSetting>();
    public DbSet<History> History => Set<History>();
    public DbSet<Material> Material => Set<Material>();



    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasKey(e => e.UserID);
        modelBuilder.Entity<RememberUser>().HasKey(e=> e.Id);
        modelBuilder.Entity<ErrorMaster>().HasKey(e => e.ID);
        modelBuilder.Entity<PLCSetting>().HasKey(e => e.Name);
        modelBuilder.Entity<PrinterSetting>().HasKey(e => e.ModelName);
        modelBuilder.Entity<Material>().HasKey(e => new { e.ModelName, e.MaterialCode });
        modelBuilder.Entity<History>().HasKey(e => e.Id);
    }
}
