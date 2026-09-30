using Microsoft.AspNetCore.Authorization;

namespace Shared.Framework.Authorization.Roles;

public sealed class RoleRequirement(string[] roles) : IAuthorizationRequirement
{
    public IReadOnlyList<string> Roles { get; } = roles;
}