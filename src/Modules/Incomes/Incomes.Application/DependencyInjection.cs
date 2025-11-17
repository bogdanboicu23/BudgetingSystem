using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Incomes.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIncomesApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}