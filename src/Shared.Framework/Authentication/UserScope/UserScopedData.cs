using System.Security.Claims;

namespace Shared.Framework.Authentication.UserScope;

public sealed class UserScopedData
{
    public Guid? Id { get; private set; }

    public string? Email { get; private set; }

    public string? UserName { get; private set; }

    public string? AuthMethod { get; private set; }

    public IReadOnlyList<string> Roles { get; private set; } = [];

    public IReadOnlyList<string> Permissions { get; private set; } = [];

    public bool IsAuthenticated => Id.HasValue;

    public Guid RequireId() => Id ?? throw new InvalidOperationException("CurrentUser is not authenticated. Did you forget [Authorize] on the endpoint?");

    public bool HasPermission(string permission) =>
        Permissions.Contains(permission, StringComparer.Ordinal);

    public bool HasRole(string role) =>
        Roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));

    internal void Hydrate(ClaimsPrincipal principal, IPermissionResolver permissionResolver)
    {
        if (principal.Identity?.IsAuthenticated != true)
            return;

        string? sub = principal.FindFirstValue(AuthClaimTypes.SUB);
        if (Guid.TryParse(sub, out Guid id))
            Id = id;

        Email = principal.FindFirstValue(AuthClaimTypes.EMAIL);
        UserName = principal.FindFirstValue(AuthClaimTypes.NAME);
        AuthMethod = principal.FindFirstValue(AuthClaimTypes.AUTH_METHOD);

        Roles = principal.Claims
            .Where(c => string.Equals(c.Type, AuthClaimTypes.ROLE, StringComparison.Ordinal))
            .Select(c => c.Value)
            .ToArray();

        Permissions = permissionResolver.ResolvePermissions(Roles).ToArray();
    }
}