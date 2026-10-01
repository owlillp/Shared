using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Authentication.Authentication;
using Shared.Authentication.Authentication.UserScope;
using Shared.Authentication.Authorization.Permissions;
using Shared.Authentication.Authorization.Roles;

namespace Shared.Authentication;

/// <summary>
/// Настройка JWT Bearer-аутентификации и авторизации по ролям и правам.
/// </summary>
public static class AuthenticationExtensions
{
    public static IServiceCollection AddPlatformAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IReadOnlyDictionary<string, IReadOnlyList<string>> rolePermissions,
        string sectionName = BearerTokenSettings.SECTION_NAME)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        services.AddBearerAuthentication(configuration, sectionName);
        services.AddAuthorization();

        services.AddSingleton(rolePermissions);
        services.AddSingleton<IPermissionResolver, RolePermissionResolver>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
        services.AddUserScopedData();

        return services;
    }

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
                $"'{sectionName}:Authority' is not configured. "
                + $"Set {sectionName}:Authority in appsettings or the {sectionName.ToUpperInvariant()}__AUTHORITY env var.");
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

    public static IApplicationBuilder UsePlatformAuthentication(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseMiddleware<UserScopedDataMiddleware>();
        app.UseAuthorization();

        return app;
    }
}