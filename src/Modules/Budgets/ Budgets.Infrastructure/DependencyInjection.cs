using Budgets.Domain.Repositories;
using Budgets.Infrastructure.Data;
using Budgets.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Budgets.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBudgetsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var databaseProvider = configuration["DatabaseProvider"] ?? "SQLite";

        services.AddDbContext<BudgetDbContext>(options =>
        {
            if (databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
            {
                options.UseNpgsql(connectionString,
                    b => b.MigrationsAssembly(typeof(BudgetDbContext).Assembly.FullName));
            }
            else
            {
                // For now, let's just use PostgreSQL since SQLite package might not be available
                options.UseNpgsql(connectionString ?? "Host=localhost;Database=budget_system;Username=postgres;Password=postgres;Port=5432",
                    b => b.MigrationsAssembly(typeof(BudgetDbContext).Assembly.FullName));
            }

            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        services.AddScoped<IBudgetRepository, BudgetRepository>();
        services.AddScoped<BudgetDbSeeder>();

        return services;
    }
}