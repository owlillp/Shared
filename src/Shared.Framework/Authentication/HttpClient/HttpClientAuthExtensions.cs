using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shared.Framework.Authentication.HttpClient;

public static class HttpClientAuthExtensions
{
    public static IServiceCollection AddServiceTokenForwarding(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<ServiceTokenOptions>(configuration.GetSection(ServiceTokenOptions.SECTION_NAME));

        services.AddHttpContextAccessor();

        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<ServiceTokenProvider>();
        services.AddTransient<TokenForwardingHandler>();

        return services;
    }
}