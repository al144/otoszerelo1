using Microsoft.EntityFrameworkCore;
using otoszerelo.Data;

namespace otoszerelo;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddDbContext<WorkshopDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("Workshop")
                              ?? "Data Source=otoszerelo.db"));

        var app = builder.Build();

        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<WorkshopDbContext>();
            await db.Database.EnsureCreatedAsync();
            await SeedData.InitializeAsync(db);
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapRazorPages()
            .WithStaticAssets();

        await app.RunAsync();
    }
}