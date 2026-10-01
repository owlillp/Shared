using Microsoft.AspNetCore.Authorization;

namespace Shared.Authentication.Authorization.Roles;

/// <summary>
/// Требование авторизации по одной из ролей.
/// </summary>
public sealed class RoleRequirement(string[] roles) : IAuthorizationRequirement
{
    public IReadOnlyList<string> Roles { get; } = roles;
}