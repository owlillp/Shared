using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.Framework.Authentication;

namespace Shared.Framework.Authorization;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddPermissionAuthorization(
        this IServiceCollection services,
        IReadOnlyDictionary<string, IReadOnlyList<string>> rolePermissions,
        string authenticationScheme)
    {
        services.AddAuthorization();

        services.AddSingleton<IPermissionResolver, RolePermissionResolver>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }

    public static RouteHandlerBuilder RequirePermissions(
        this RouteHandlerBuilder builder,
        params string[] permissions)
    {
        string[] policyNames = permissions
            .Select(permission => $"{PermissionPolicyProvider.PERMISSION_PREFIX}{permission}")
            .ToArray();

        return builder.RequireAuthorization(policyNames);
    }
}