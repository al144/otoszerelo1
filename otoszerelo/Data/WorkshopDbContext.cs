using Microsoft.EntityFrameworkCore;
using otoszerelo.Models;

namespace otoszerelo.Data;

public class WorkshopDbContext(DbContextOptions<WorkshopDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Repair> Repairs => Set<Repair>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasMany(customer => customer.Repairs)
            .WithOne(repair => repair.Customer)
            .HasForeignKey(repair => repair.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Repair>()
            .Property(repair => repair.Status)
            .HasConversion<string>();
    }
}
