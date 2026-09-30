using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Framework.Authorization;

namespace Shared.Framework.Authentication;

public static class BearerAuthenticationExtensions
{
    public static AuthenticationBuilder AddBearerAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = BearerTokenSettings.SECTION_NAME)
    {
        var section = configuration.GetSection(sectionName);
        var settings = section.Get<BearerTokenSettings>() ?? new BearerTokenSettings();

        if (string.IsNullOrWhiteSpace(settings.Authority))
        {
            throw new InvalidOperationException(
                $"'{sectionName}:SigningKey' is not configured. "
                + $"Set {sectionName}:SigningKey in appsettings or the JWT__SIGNINGKEY env var.");
        }

        return services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = settings.Authority;
                options.RequireHttpsMetadata = settings.Authority.StartsWith("https", StringComparison.OrdinalIgnoreCase);
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = settings.Audiences.Count > 0,
                    ValidAudiences = settings.Audiences,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = AuthClaimTypes.NAME,
                    RoleClaimType = AuthClaimTypes.ROLE,
                };
            });
    }

    public static IServiceCollection AddPlatformAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IReadOnlyDictionary<string, IReadOnlyList<string>> rolePermissions,
        string sectionName = BearerTokenSettings.SECTION_NAME)
    {
        services.AddBearerAuthentication(configuration, sectionName);
        services.AddPermissionAuthorization(rolePermissions, JwtBearerDefaults.AuthenticationScheme);
        services.AddCurrentUser();

        return services;
    }
}