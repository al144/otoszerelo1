using Microsoft.EntityFrameworkCore;
using otoszerelo.Models;

namespace otoszerelo.Data;

public static class SeedData
{
    public static async Task InitializeAsync(WorkshopDbContext db)
    {
        if (await db.Customers.AnyAsync())
        {
            return;
        }

        var customers = new[]
        {
            new Customer { Name = "Kovács Anna", Phone = "+36 30 123 4567", Email = "anna.kovacs@example.com" },
            new Customer { Name = "Nagy Péter", Phone = "+36 20 234 5678", Email = "peter.nagy@example.com" },
            new Customer { Name = "Tóth Eszter", Phone = "+36 70 345 6789", Email = "eszter.toth@example.com" }
        };
        db.Customers.AddRange(customers);
        await db.SaveChangesAsync();

        db.Repairs.AddRange(
            new Repair
            {
                CustomerId = customers[0].Id,
                LicensePlate = "ABC-123",
                Vehicle = "Toyota Corolla",
                Description = "Olajcsere és éves átvizsgálás",
                Cost = 42000,
                Status = RepairStatus.Completed,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Repair
            {
                CustomerId = customers[1].Id,
                LicensePlate = "DEF-456",
                Vehicle = "Škoda Octavia",
                Description = "Első fékbetétek cseréje",
                Status = RepairStatus.InProgress,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Repair
            {
                CustomerId = customers[2].Id,
                LicensePlate = "GHI-789",
                Vehicle = "Suzuki Swift",
                Description = "Futómű zajának diagnosztikája",
                Status = RepairStatus.Received,
                CreatedAt = DateTime.UtcNow
            });
        await db.SaveChangesAsync();
    }
}
