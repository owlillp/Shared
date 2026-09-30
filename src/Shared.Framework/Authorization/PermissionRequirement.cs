using Microsoft.AspNetCore.Authorization;

namespace Shared.Framework.Authorization;

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}