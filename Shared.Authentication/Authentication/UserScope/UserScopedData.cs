namespace Shared.Authentication.Authentication.UserScope;

/// <summary>
/// Данные текущего пользователя из claims (id, роли, права), заполняемые middleware на запрос.
/// </summary>
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

    internal void Authenticate(
        Guid? id,
        string? name,
        string? email,
        string? authMethod,
        IReadOnlyList<string> roles,
        IReadOnlyList<string> permissions)
    {
       Id = id;
       UserName = name;
       Email = email;
       AuthMethod = authMethod;
       Roles = roles;
       Permissions = permissions;
    }
}