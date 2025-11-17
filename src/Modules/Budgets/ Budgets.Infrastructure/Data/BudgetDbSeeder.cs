using Budgets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Budgets.Infrastructure.Data;

public class BudgetDbSeeder
{
    private readonly BudgetDbContext _context;
    private readonly ILogger<BudgetDbSeeder> _logger;

    public BudgetDbSeeder(BudgetDbContext context, ILogger<BudgetDbSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Try to create the database, but handle gracefully if database server isn't available
            try
            {
                await _context.Database.EnsureCreatedAsync();
            }
            catch (Exception dbEx)
            {
                _logger.LogWarning(dbEx, "Database server not available. Skipping database seeding.");
                return;
            }

            if (!await _context.Budgets.AnyAsync())
            {
                _logger.LogInformation("Seeding initial budget data...");

                var budget1 = new Budget(Guid.NewGuid(), "November 2024 Budget",
                    new DateTime(2024, 11, 1), new DateTime(2024, 11, 30));

                budget1.AddCategory("Housing", 1200m);
                budget1.AddCategory("Food", 400m);
                budget1.AddCategory("Transportation", 300m);
                budget1.AddCategory("Entertainment", 200m);
                budget1.AddCategory("Utilities", 150m);

                var budget2 = new Budget(Guid.NewGuid(), "Weekly Groceries Budget",
                    new DateTime(2024, 11, 11), new DateTime(2024, 11, 17));

                budget2.AddCategory("Groceries", 100m);
                budget2.AddCategory("Household Items", 50m);

                var budget3 = new Budget(Guid.NewGuid(), "Holiday Savings",
                    new DateTime(2024, 11, 1), new DateTime(2024, 12, 25));

                budget3.AddCategory("Gifts", 800m);
                budget3.AddCategory("Travel", 500m);
                budget3.AddCategory("Food & Entertainment", 300m);

                await _context.Budgets.AddRangeAsync(budget1, budget2, budget3);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Initial budget data seeded successfully!");
            }
            else
            {
                _logger.LogInformation("Database already contains budget data, skipping seeding.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}