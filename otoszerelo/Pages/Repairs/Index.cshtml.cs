using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using otoszerelo.Data;
using otoszerelo.Models;

namespace otoszerelo.Pages.Repairs;

public class IndexModel(WorkshopDbContext db) : PageModel
{
    [BindProperty]
    public Repair Repair { get; set; } = new();

    [BindProperty]
    public Customer NewCustomer { get; set; } = new();

    public IReadOnlyList<Repair> Repairs { get; private set; } = [];
    public string? StatusFilter { get; private set; }

    public async Task OnGetAsync(string? status)
    {
        StatusFilter = ParseStatus(status);
        await LoadPageDataAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        Repair.LicensePlate = Repair.LicensePlate?.Trim().ToUpperInvariant() ?? string.Empty;
        Repair.Vehicle = Repair.Vehicle?.Trim() ?? string.Empty;
        Repair.Description = Repair.Description?.Trim() ?? string.Empty;
        NewCustomer.Name = NewCustomer.Name?.Trim() ?? string.Empty;
        NewCustomer.Phone = string.IsNullOrWhiteSpace(NewCustomer.Phone) ? null : NewCustomer.Phone.Trim();
        NewCustomer.Email = string.IsNullOrWhiteSpace(NewCustomer.Email) ? null : NewCustomer.Email.Trim();

        if (Repair.Cost is <= 0)
        {
            ModelState.AddModelError("Repair.Cost", "A költségnek nullánál nagyobbnak kell lennie.");
        }

        if (Repair.Status == RepairStatus.Completed && Repair.Cost is null or <= 0)
        {
            ModelState.AddModelError("Repair.Cost", "Kész állapothoz adj meg nullánál nagyobb költséget.");
        }

        ModelState.Remove("Repair.CustomerId");

        if (!ModelState.IsValid)
        {
            await LoadPageDataAsync();
            return Page();
        }

        await using var transaction = await db.Database.BeginTransactionAsync();
        db.Customers.Add(NewCustomer);
        await db.SaveChangesAsync();
        Repair.CustomerId = NewCustomer.Id;

        Repair.CreatedAt = DateTime.UtcNow;
        db.Repairs.Add(Repair);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        TempData["Notice"] = "A javítást sikeresen rögzítettük.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostStatusAsync(int id, RepairStatus status, string? statusFilter)
    {
        var repair = await db.Repairs.FindAsync(id);
        if (repair is null)
        {
            TempData["Error"] = "A javítás nem található.";
        }
        else if (!Enum.IsDefined(status))
        {
            TempData["Error"] = "A megadott állapot érvénytelen.";
        }
        else if (status == RepairStatus.Completed && repair.Cost is null or <= 0)
        {
            TempData["Error"] = "A javítás csak nullánál nagyobb költség megadásával zárható le. Szerkeszd a munkalapot!";
        }
        else
        {
            repair.Status = status;
            await db.SaveChangesAsync();
            TempData["Notice"] = "A javítás állapotát frissítettük.";
        }

        return RedirectToPage(new { status = ParseStatus(statusFilter) });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, string? statusFilter)
    {
        var repair = await db.Repairs.FindAsync(id);
        if (repair is null)
        {
            TempData["Error"] = "A törölni kívánt javítás nem található.";
        }
        else
        {
            db.Repairs.Remove(repair);
            await db.SaveChangesAsync();
            TempData["Notice"] = "A javítást töröltük.";
        }

        return RedirectToPage(new { status = ParseStatus(statusFilter) });
    }

    private async Task LoadPageDataAsync()
    {
        var query = db.Repairs.Include(repair => repair.Customer).AsQueryable();
        if (StatusFilter is not null && Enum.TryParse<RepairStatus>(StatusFilter, out var status))
        {
            query = query.Where(repair => repair.Status == status);
        }

        Repairs = await query.OrderByDescending(repair => repair.CreatedAt).ToListAsync();
    }

    private static string? ParseStatus(string? status)
    {
        return Enum.TryParse<RepairStatus>(status, ignoreCase: true, out var parsed)
               && Enum.IsDefined(parsed)
            ? parsed.ToString()
            : null;
    }
}
