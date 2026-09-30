using Shared.Framework.Authentication;

namespace Shared.Framework.Authorization;

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