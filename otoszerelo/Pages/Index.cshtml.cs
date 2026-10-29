using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using otoszerelo.Data;
using otoszerelo.Models;

namespace otoszerelo.Pages;

public class IndexModel(WorkshopDbContext db) : PageModel
{
    public int CustomerCount { get; private set; }
    public int OpenRepairCount { get; private set; }
    public int CompletedRepairCount { get; private set; }
    public IReadOnlyList<Repair> RecentRepairs { get; private set; } = [];

    public async Task OnGetAsync()
    {
        CustomerCount = await db.Customers.CountAsync();
        OpenRepairCount = await db.Repairs.CountAsync(repair => repair.Status != RepairStatus.Completed);
        CompletedRepairCount = await db.Repairs.CountAsync(repair => repair.Status == RepairStatus.Completed);
        RecentRepairs = await db.Repairs
            .Include(repair => repair.Customer)
            .OrderByDescending(repair => repair.CreatedAt)
            .Take(5)
            .ToListAsync();
    }
}
