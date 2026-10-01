using Microsoft.AspNetCore.Authorization;
using Shared.Authentication.Authentication.UserScope;

namespace Shared.Authentication.Authorization.Roles;

/// <summary>
/// Проверяет, что у пользователя есть хотя бы одна из требуемых ролей.
/// </summary>
public sealed class RoleAuthorizationHandler(UserScopedData userScopedData) : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        bool hasRequiredRole = requirement.Roles.Any(userScopedData.HasRole);
        if (hasRequiredRole)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}