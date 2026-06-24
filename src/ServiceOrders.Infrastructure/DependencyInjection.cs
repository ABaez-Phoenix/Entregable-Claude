using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceOrders.Domain.Interfaces;
using ServiceOrders.Infrastructure.Persistence;
using ServiceOrders.Infrastructure.Repositories;

namespace ServiceOrders.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IServiceOrderRepository, ServiceOrderRepository>();

        return services;
    }
}
