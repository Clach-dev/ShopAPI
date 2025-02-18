using Domain.Interfaces.IAlgorithms;
using Domain.Interfaces.IRepositories;
using Infrastructure.Algorithms;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDatabase(configuration)
            .AddRepositories();
    }
    
    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SqlConnectionString");
        
        return services.AddDbContext<ShopDbContext>(options =>
            options
                .UseSqlServer(connectionString)
                .UseLazyLoadingProxies());
    }
    
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IOrderItemRepository, OrderItemRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>();
    }
    
    private static IServiceCollection AddAlgorithms(this IServiceCollection services)
    {
        return services
            .AddScoped<ITokensGenerator, TokensGenerator>()
            .AddScoped<IPasswordHasher, PasswordHasher>();
    }
}