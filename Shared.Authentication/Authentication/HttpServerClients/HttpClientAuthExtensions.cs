using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Authentication.Authentication.HttpServerClients;

/// <summary>
/// Регистрирует провайдер сервисного токена и forwarding-handler для исходящих HTTP-запросов.
/// </summary>
public static class HttpClientAuthExtensions
{
    public static IServiceCollection AddServiceTokenForwarding(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ServiceTokenOptions>(configuration.GetSection(ServiceTokenOptions.SECTION_NAME));

        services.AddHttpContextAccessor();

        services.AddHttpClient("ServiceTokenProvider");
        services.AddSingleton<ServiceTokenProvider>();
        services.AddTransient<TokenForwardingHandler>();

        return services;
    }
}