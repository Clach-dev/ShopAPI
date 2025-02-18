using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddMapper()
            .AddMediatR();
    }

    private static IServiceCollection AddMapper(this IServiceCollection services)
    {
        return services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        return services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
    }
}