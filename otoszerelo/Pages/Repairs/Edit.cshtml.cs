using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using otoszerelo.Data;
using otoszerelo.Models;

namespace otoszerelo.Pages.Repairs;

public class EditModel(WorkshopDbContext db) : PageModel
{
    [BindProperty]
    public Repair Repair { get; set; } = new();

    public IReadOnlyList<Customer> Customers { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var repair = await db.Repairs.FindAsync(id);
        if (repair is null)
        {
            return NotFound();
        }

        Repair = repair;
        await LoadCustomersAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Repair.LicensePlate = Repair.LicensePlate?.Trim().ToUpperInvariant() ?? string.Empty;
        Repair.Vehicle = Repair.Vehicle?.Trim() ?? string.Empty;
        Repair.Description = Repair.Description?.Trim() ?? string.Empty;

        if (Repair.Cost is <= 0)
        {
            ModelState.AddModelError("Repair.Cost", "A költségnek nullánál nagyobbnak kell lennie.");
        }

        if (Repair.Status == RepairStatus.Completed && Repair.Cost is null or <= 0)
        {
            ModelState.AddModelError("Repair.Cost", "Kész állapothoz adj meg nullánál nagyobb költséget.");
        }

        if (!ModelState.IsValid)
        {
            await LoadCustomersAsync();
            return Page();
        }

        var existing = await db.Repairs.FindAsync(Repair.Id);
        if (existing is null)
        {
            return NotFound();
        }

        if (!await db.Customers.AnyAsync(customer => customer.Id == Repair.CustomerId))
        {
            ModelState.AddModelError("Repair.CustomerId", "A kiválasztott ügyfél nem található.");
            await LoadCustomersAsync();
            return Page();
        }

        existing.CustomerId = Repair.CustomerId;
        existing.LicensePlate = Repair.LicensePlate;
        existing.Vehicle = Repair.Vehicle;
        existing.Description = Repair.Description;
        existing.Cost = Repair.Cost;
        existing.Status = Repair.Status;
        await db.SaveChangesAsync();
        TempData["Notice"] = "A javítás adatait frissítettük.";
        return RedirectToPage("Index");
    }

    private async Task LoadCustomersAsync()
    {
        Customers = await db.Customers.OrderBy(customer => customer.Name).ToListAsync();
    }
}
