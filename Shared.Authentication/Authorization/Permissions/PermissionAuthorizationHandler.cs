using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Shared.Authentication.Authentication.UserScope;

namespace Shared.Authentication.Authorization.Permissions;

/// <summary>
/// Проверяет наличие требуемого права у текущего пользователя.
/// </summary>
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