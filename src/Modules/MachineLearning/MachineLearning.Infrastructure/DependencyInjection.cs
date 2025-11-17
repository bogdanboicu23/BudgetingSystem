using System;
using System.Collections.Generic;
using MachineLearning.Domain.Pipelines;
using MachineLearning.Domain.Services;
using MachineLearning.Domain.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace MachineLearning.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMachineLearningInfrastructure(this IServiceCollection services)
    {
        // Register strategies
        services.AddScoped<RuleBasedClassificationStrategy>();
        services.AddScoped<MlNetClassificationStrategy>();

        // Register all strategies for ensemble
        services.AddScoped<IEnumerable<ITransactionClassificationStrategy>>(provider =>
            new List<ITransactionClassificationStrategy>
            {
                provider.GetRequiredService<RuleBasedClassificationStrategy>(),
                provider.GetRequiredService<MlNetClassificationStrategy>()
            });

        // Register pipeline components
        services.AddScoped<ITransactionFeatureExtractor, TransactionFeatureExtractor>();

        // Register main service
        services.AddScoped<ITransactionCategorizationService, TransactionCategorizationService>();

        return services;
    }
}