using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Shared.Authentication.Authentication.UserScope;

/// <summary>
/// Заполняет <see cref="UserScopedData"/> из ClaimsPrincipal текущего запроса.
/// </summary>
public sealed class UserScopedDataMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(
        HttpContext context,
        UserScopedData userScopedData,
        IPermissionResolver permissionResolver)
    {
        HydrateUserScopedData(userScopedData, context.User, permissionResolver);
        return next(context);
    }

    private static void HydrateUserScopedData(
        UserScopedData userScopedData,
        ClaimsPrincipal principal,
        IPermissionResolver permissionResolver)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return;
        }

        string? sub = principal.FindFirstValue(AuthClaimTypes.SUB);

        Guid? id = Guid.TryParse(sub, out Guid value) ? value : null;
        string? email = principal.FindFirstValue(AuthClaimTypes.EMAIL);
        string? userName = principal.FindFirstValue(AuthClaimTypes.NAME);
        string? authMethod = principal.FindFirstValue(AuthClaimTypes.AUTH_METHOD);

        string[] roles = principal.Claims
            .Where(c => string.Equals(c.Type, AuthClaimTypes.ROLE, StringComparison.Ordinal))
            .Select(c => c.Value)
            .ToArray();

        string[] permissions = permissionResolver
            .ResolvePermissions(roles)
            .ToArray();

        userScopedData.Authenticate(id, userName, email, authMethod, roles, permissions);
    }
}