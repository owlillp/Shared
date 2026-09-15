using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace Shared.Framework.OpenApi;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiSpec(this IServiceCollection services, string title, string version)
    {
        services.AddOpenApi(version, options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = title,
                    Version = version,
                };
                return Task.CompletedTask;
            });
        });

        return services;
    }
}