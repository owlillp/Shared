using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shared.Framework.Authentication.UserScope;

public sealed class UserScopedDataMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(
        HttpContext context,
        UserScopedData userScopedData,
        IPermissionResolver permissionResolver)
    {
        userScopedData.Hydrate(context.User, permissionResolver);
        return next(context);
    }
}

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