using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using otoszerelo.Data;
using otoszerelo.Models;

namespace otoszerelo.Pages.Customers;

public class CreateModel(WorkshopDbContext db) : PageModel
{
    [BindProperty]
    public Customer Customer { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Customer.Name = Customer.Name.Trim();
        Customer.Phone = string.IsNullOrWhiteSpace(Customer.Phone) ? null : Customer.Phone.Trim();
        Customer.Email = string.IsNullOrWhiteSpace(Customer.Email) ? null : Customer.Email.Trim();
        db.Customers.Add(Customer);
        await db.SaveChangesAsync();
        TempData["Notice"] = "Az ügyfelet sikeresen létrehoztuk.";
        return RedirectToPage("Index");
    }
}
