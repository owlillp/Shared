using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shared.Authentication.Authentication.UserScope;

/// <summary>
/// Регистрация и подключение middleware данных текущего пользователя.
/// </summary>
public static class UserScopedDataExtensions
{
    public static IServiceCollection AddUserScopedData(this IServiceCollection services)
    {
        services.AddScoped<UserScopedData>();
        services.TryAddSingleton<IPermissionResolver, EmptyPermissionResolver>();
        return services;
    }

    public static IApplicationBuilder UseUserScopedData(this IApplicationBuilder app)
        => app.UseMiddleware<UserScopedDataMiddleware>();
}