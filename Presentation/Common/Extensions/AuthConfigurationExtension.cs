using System.Text;
using Domain.Constants;
using Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Common.Extensions;

public static class AuthConfigurationExtension
{
    public static IServiceCollection AddPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.OnlyAdminAccess, policy =>
                policy.RequireRole(Roles.Admin.ToString()));
            
            options.AddPolicy(Policies.OnlyUserAccess, policy =>
                policy.RequireRole(Roles.User.ToString()));
            
            options.AddPolicy(Policies.AuthenticateAccess, policy =>
                policy.RequireAssertion(_ => true));
        });

        return services;
    }
    
    public static IServiceCollection AddJwtValidation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]
                        ?? throw new InvalidOperationException()))
                };
            });
        
        return services;
    }
    
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        return services.AddCors(options =>
        {
            options.AddPolicy("AllowAngular", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Content-Disposition")  
                    .SetIsOriginAllowed(_ => true);  
            });
        });
    }
}