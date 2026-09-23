namespace Shared.Framework.Authentication;

public interface IPermissionResolver
{
    IReadOnlyCollection<string> ResolvePermissions(IEnumerable<string> roles);
}

internal sealed class EmptyPermissionResolver : IPermissionResolver
{
    public IReadOnlyCollection<string> ResolvePermissions(IEnumerable<string> roles) => [];
}
