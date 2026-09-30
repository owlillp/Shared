using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Shared.Framework.Authorization;

public sealed class PermissionPolicyProvider(
    IOptions<AuthorizationOptions> options,
    string authenticationScheme) : IAuthorizationPolicyProvider
{
    public const string PERMISSION_PREFIX = "Permission:";

    private readonly DefaultAuthorizationPolicyProvider _fallback = new (options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        ArgumentNullException.ThrowIfNull(policyName);

        if (policyName.StartsWith(PERMISSION_PREFIX, StringComparison.Ordinal))
        {
            string permission = policyName[PERMISSION_PREFIX.Length..];

            AuthorizationPolicy policy = new AuthorizationPolicyBuilder(authenticationScheme)
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permission))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallback.GetPolicyAsync(policyName);
    }
}