using Shared.Authentication.Authentication.UserScope;

namespace Shared.Authentication.Authorization.Roles;

/// <summary>
/// Сопоставляет роли с правами по переданной статической карте.
/// </summary>
public sealed class RolePermissionResolver(IReadOnlyDictionary<string, IReadOnlyList<string>> rolePermissions) : IPermissionResolver
{
    public IReadOnlyCollection<string> ResolvePermissions(IEnumerable<string> roles)
    {
        ArgumentNullException.ThrowIfNull(roles);

        var permissions = new HashSet<string>(StringComparer.Ordinal);
        foreach (string role in roles)
        {

            if (rolePermissions.TryGetValue(role, out IReadOnlyList<string>? value))
            {
                permissions.UnionWith(value);
            }
        }

        return permissions;
    }
}