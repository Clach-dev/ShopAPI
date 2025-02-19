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
                    ValidAudience = configuration["JwtSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]
                        ?? throw new InvalidOperationException()))
                };
            });
        
        return services;
    }
}