using Microsoft.EntityFrameworkCore;
using Real_Estate_Laba.Models;

namespace Real_Estate_Laba.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Property>()
             .HasOne(x => x.Location)
             .WithOne()
             .HasForeignKey<Property>(x => x.Id)
             .OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<Image> Images { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Property> Properties { get; set; }
}
