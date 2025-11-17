using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Incomes.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIncomesInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add repository registrations when implemented

        return services;
    }
}