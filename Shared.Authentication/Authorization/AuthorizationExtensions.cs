using Microsoft.AspNetCore.Builder;
using Shared.Authentication.Authorization.Permissions;

namespace Shared.Authentication.Authorization;

/// <summary>
/// Extension-методы политик авторизации для endpoints: права, роли и анонимный доступ.
/// </summary>
public static class AuthorizationExtensions
{
    public static TBuilder AllowAnonymousEndpoint<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.AllowAnonymous();
        return builder;
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

    public static RouteHandlerBuilder RequireAnyRole(
        this RouteHandlerBuilder builder,
        params string[] roles)
    {
        string policyName = $"{PermissionPolicyProvider.ROLE_POLICY_PREFIX}{string.Join(',', roles)}";
        builder.RequireAuthorization(policyName);

        return builder;
    }
}