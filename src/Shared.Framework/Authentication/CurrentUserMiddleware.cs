using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shared.Framework.Authentication;

public sealed class CurrentUserMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(
        HttpContext context,
        CurrentUser currentUser,
        IPermissionResolver permissionResolver)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(currentUser);

        currentUser.Hydrate(context.User, permissionResolver);

        return next(context);
    }
}

public static class CurrentUserExtensions
{
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped<CurrentUser>();
        services.TryAddSingleton<IPermissionResolver, EmptyPermissionResolver>();
        return services;
    }

    public static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<CurrentUserMiddleware>();
    }
}