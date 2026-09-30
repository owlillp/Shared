using Microsoft.AspNetCore.Authorization;
using Shared.Framework.Authentication.UserScope;

namespace Shared.Framework.Authorization.Roles;

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