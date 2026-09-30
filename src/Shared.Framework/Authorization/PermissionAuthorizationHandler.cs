using Microsoft.AspNetCore.Authorization;
using Shared.Framework.Authentication;

namespace Shared.Framework.Authorization;

public sealed class PermissionAuthorizationHandler(IPermissionResolver permissionResolver) : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        IEnumerable<string> roles = context.User
            .FindAll(AuthClaimTypes.ROLE)
            .Select(claim => claim.Value);

        var permissions = permissionResolver.ResolvePermissions(roles);
        if (permissions.Contains(requirement.Permission, StringComparer.Ordinal))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}