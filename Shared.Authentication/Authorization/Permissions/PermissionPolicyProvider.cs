using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Shared.Authentication.Authorization.Roles;

namespace Shared.Authentication.Authorization.Permissions;

/// <summary>
/// Динамически создаёт политики авторизации по префиксам прав и ролей.
/// </summary>
public sealed class PermissionPolicyProvider(
    IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    public const string PERMISSION_PREFIX = "Permission:";
    public const string ROLE_POLICY_PREFIX = "AnyRole:";

    private readonly DefaultAuthorizationPolicyProvider _fallback = new (options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        ArgumentNullException.ThrowIfNull(policyName);

        if (policyName.StartsWith(PERMISSION_PREFIX, StringComparison.Ordinal))
        {
            string permission = policyName[PERMISSION_PREFIX.Length..];

            AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permission))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        if (policyName.StartsWith(ROLE_POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
        {
            string[] roles = policyName[ROLE_POLICY_PREFIX.Length..]
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new RoleRequirement(roles))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallback.GetPolicyAsync(policyName);
    }
}