namespace Shared.Authentication.Authentication.UserScope;

/// <summary>
/// Преобразует роли пользователя в набор разрешений.
/// </summary>
public interface IPermissionResolver
{
    IReadOnlyCollection<string> ResolvePermissions(IEnumerable<string> roles);
}

/// <summary>
/// Заглушка <see cref="IPermissionResolver"/>, возвращающая пустой набор прав.
/// </summary>
internal sealed class EmptyPermissionResolver : IPermissionResolver
{
    public IReadOnlyCollection<string> ResolvePermissions(IEnumerable<string> roles) => [];
}
