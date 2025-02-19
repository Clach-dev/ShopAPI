using Application;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Presentation.Common.Middleware;

namespace Presentation.Common.Extensions;

public static class ProgramExtension
{
    public static WebApplicationBuilder CreateBuilder(string[] args)
    {
        return WebApplication.CreateBuilder(args);
    }

    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddInfrastructure(builder.Configuration)
            .AddApplication();
        
        builder.Services
            .AddPolicies()
            .AddJwtValidation(builder.Configuration);
        
        builder.Services
            .AddHttpContextAccessor()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
        
        builder.Services
            .AddValidatorsFromAssemblyContaining<Program>()
            .AddFluentValidationAutoValidation();
        
        builder.Services
            .AddControllers();
        
        return builder;
    }
    
    public static WebApplication Build(this WebApplicationBuilder builder)
    {
        return builder.Build();
    }

    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        
        if (app.Environment.IsDevelopment())
        {
            app
                .UseSwagger()
                .UseSwaggerUI();
        }
        
        app
            .UseHttpsRedirection()
            .UseMiddleware<LoggingMiddleware>()
            .UseAuthentication()
            .UseAuthorization();
        
        app
            .MapControllers();

        return app;
    }

    public static void Run(this WebApplication app)
    {
        app.Run();
    }
}