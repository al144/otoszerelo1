using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using otoszerelo.Data;
using otoszerelo.Models;

namespace otoszerelo.Pages.Customers;

public class IndexModel(WorkshopDbContext db) : PageModel
{
    public IReadOnlyList<Customer> Customers { get; private set; } = [];

    public async Task OnGetAsync()
    {
        await LoadCustomersAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var customer = await db.Customers
            .Include(item => item.Repairs)
            .SingleOrDefaultAsync(item => item.Id == id);
        if (customer is null)
        {
            TempData["Error"] = "A törölni kívánt ügyfél nem található.";
        }
        else if (customer.Repairs.Count > 0)
        {
            TempData["Error"] = "Az ügyfél nem törölhető, mert javítás tartozik hozzá.";
        }
        else
        {
            db.Customers.Remove(customer);
            await db.SaveChangesAsync();
            TempData["Notice"] = "Az ügyfelet töröltük.";
        }

        return RedirectToPage();
    }

    private async Task LoadCustomersAsync()
    {
        Customers = await db.Customers
            .Include(customer => customer.Repairs)
            .OrderBy(customer => customer.Name)
            .ToListAsync();
    }
}
