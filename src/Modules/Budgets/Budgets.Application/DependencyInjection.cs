using Budgets.Application.Observers;
using Budgets.Application.Reports;
using Budgets.Application.Services;
using Budgets.Domain.Events;
using Budgets.Domain.Strategies;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Budgets.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBudgetsApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Strategy Pattern - Budget Calculation Strategies
        services.AddTransient<FixedBudgetStrategy>();
        services.AddTransient<RollingBudgetStrategy>();
        services.AddTransient<PercentageBasedBudgetStrategy>();

        // Observer Pattern - Budget Event Handling
        services.AddSingleton<BudgetSubject>();
        services.AddTransient<IBudgetObserver, EmailNotificationObserver>();
        services.AddTransient<IBudgetObserver, DatabaseLogObserver>();

        // Template Method Pattern - Report Generation
        services.AddTransient<SummaryReportGenerator>();
        services.AddTransient<DetailedReportGenerator>();

        // Application Services
        services.AddScoped<BudgetService>();

        // Configure observers
        services.AddSingleton(provider =>
        {
            var subject = provider.GetRequiredService<BudgetSubject>();
            var observers = provider.GetServices<IBudgetObserver>();

            foreach (var observer in observers)
            {
                subject.Subscribe(observer);
            }

            return subject;
        });

        return services;
    }
}