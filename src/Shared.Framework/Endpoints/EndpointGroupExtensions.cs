using System.Reflection;
using Microsoft.AspNetCore.Routing;

namespace Shared.Framework.Endpoints;

public static class EndpointGroupExtensions
{
    public static IEndpointRouteBuilder MapEndpointGroups(
        this IEndpointRouteBuilder app,
        Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (Type type in FindGroups(assembly))
        {
            var group = (IEndpointGroup)Activator.CreateInstance(type)!;
            group.MapGroup(app);
        }

        return app;
    }

    private static IEnumerable<Type> FindGroups(Assembly assembly) =>
        assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false }
                           && typeof(IEndpointGroup).IsAssignableFrom(type));
}