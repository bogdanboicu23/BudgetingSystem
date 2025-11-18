using ExpenseService.Application.Handlers;
using ExpenseService.Domain.Repositories;
using ExpenseService.Infrastructure.Data;
using ExpenseService.Infrastructure.Repositories;

namespace ExpenseService.API.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CreateExpenseCommandHandler>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ExpenseDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IExpenseRepository, ExpenseRepository>();

        return services;
    }
}