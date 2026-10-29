using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using otoszerelo.Data;
using otoszerelo.Models;

namespace otoszerelo.Pages.Customers;

public class EditModel(WorkshopDbContext db) : PageModel
{
    [BindProperty]
    public Customer Customer { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var customer = await db.Customers.FindAsync(id);
        if (customer is null)
        {
            return NotFound();
        }

        Customer = customer;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existing = await db.Customers.FindAsync(Customer.Id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = Customer.Name.Trim();
        existing.Phone = string.IsNullOrWhiteSpace(Customer.Phone) ? null : Customer.Phone.Trim();
        existing.Email = string.IsNullOrWhiteSpace(Customer.Email) ? null : Customer.Email.Trim();
        await db.SaveChangesAsync();
        TempData["Notice"] = "Az ügyfél adatait frissítettük.";
        return RedirectToPage("Index");
    }
}
