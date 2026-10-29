using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using otoszerelo.Data;
using otoszerelo.Models;

namespace otoszerelo.Pages.Customers;

public class DetailsModel(WorkshopDbContext db) : PageModel
{
    public Customer Customer { get; private set; } = null!;
    public decimal TotalCost => Customer.Repairs.Sum(repair => repair.Cost ?? 0);

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var customer = await db.Customers
            .Include(item => item.Repairs)
            .SingleOrDefaultAsync(item => item.Id == id);
        if (customer is null)
        {
            return NotFound();
        }

        customer.Repairs = customer.Repairs.OrderByDescending(repair => repair.CreatedAt).ToList();
        Customer = customer;
        return Page();
    }
}
