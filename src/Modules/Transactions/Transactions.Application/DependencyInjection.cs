using Microsoft.Extensions.DependencyInjection;
using Transactions.Application.Services;
using Transactions.Domain.Processing;

namespace Transactions.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddTransactionsApplication(this IServiceCollection services)
    {
        // Chain of Responsibility Pattern - Transaction Processing
        services.AddTransient<TransferDetectionHandler>();
        services.AddTransient<IncomeDetectionHandler>();
        services.AddTransient<ExpenseCategorizationHandler>();
        services.AddTransient<FallbackHandler>();

        // Register the processing chain
        services.AddTransient<IBankTransactionHandler>(provider =>
        {
            var transferHandler = provider.GetRequiredService<TransferDetectionHandler>();
            var incomeHandler = provider.GetRequiredService<IncomeDetectionHandler>();
            var expenseHandler = provider.GetRequiredService<ExpenseCategorizationHandler>();
            var fallbackHandler = provider.GetRequiredService<FallbackHandler>();

            transferHandler
                .SetNext(incomeHandler)
                .SetNext(expenseHandler)
                .SetNext(fallbackHandler);

            return transferHandler;
        });

        // Bank API and Processing Services
        services.AddScoped<IBankApiService, MockBankApiService>();
        services.AddScoped<BankTransactionProcessor>();

        return services;
    }
}