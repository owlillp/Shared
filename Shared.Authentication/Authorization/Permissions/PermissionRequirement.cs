using Microsoft.AspNetCore.Authorization;

namespace Shared.Authentication.Authorization.Permissions;

/// <summary>
/// Требование авторизации по конкретному праву.
/// </summary>
public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}