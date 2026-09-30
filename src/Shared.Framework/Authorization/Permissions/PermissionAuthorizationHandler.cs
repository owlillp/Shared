using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Shared.Framework.Authentication.UserScope;

namespace Shared.Framework.Authorization.Permissions;

public sealed class PermissionAuthorizationHandler(
    UserScopedData userScopedData,
    ILogger<PermissionAuthorizationHandler> logger) : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (userScopedData.HasPermission(requirement.Permission))
        {
            context.Succeed(requirement);
        }
        else if (userScopedData.IsAuthenticated)
        {
            logger.LogWarning(
                "Permission denied: User {UserId} less permission {Permission}",
                userScopedData.RequireId(),
                requirement.Permission);
        }

        return Task.CompletedTask;
    }
}