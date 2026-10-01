using Microsoft.Extensions.Configuration;
using Scalar.AspNetCore;

namespace Shared.Authentication;

/// <summary>
/// Extension для настройки Scalar UI с OAuth2 (OpenIddict).
/// </summary>
public static class ScalarExtensions
{
    private const string OAUTH2_SCHEME_KEY = "OAuth2";

    /// <summary>
    /// Настраивает OAuth2 Authorization Code flow в Scalar по секции <see cref="AuthenticationOptions.SECTION_NAME"/>.
    /// Если <see cref="AuthenticationOptions.AuthorizationUrl"/>, <see cref="AuthenticationOptions.TokenUrl"/>
    /// или client id не заданы — flow не подключается.
    /// </summary>
    /// <typeparam name="T">Тип опций Scalar.</typeparam>
    /// <param name="options">Опции Scalar.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Опции Scalar для chaining.</returns>
    public static T ConfigureOAuth2<T>(this T options, IConfiguration configuration)
        where T : ScalarOptions
    {
        AuthenticationOptions authOptions = configuration
            .GetSection(AuthenticationOptions.SECTION_NAME)
            .Get<AuthenticationOptions>() ?? new AuthenticationOptions();

        string? clientId = authOptions.Scalar.ClientId ?? authOptions.Audience;

        if (authOptions.AuthorizationUrl is null || authOptions.TokenUrl is null || string.IsNullOrEmpty(clientId))
        {
            return options;
        }

        options
            .AddPreferredSecuritySchemes(OAUTH2_SCHEME_KEY)
            .AddAuthorizationCodeFlow(OAUTH2_SCHEME_KEY, flow =>
            {
                flow
                    .WithClientId(clientId)
                    .WithPkce(Pkce.Sha256);

                if (!string.IsNullOrWhiteSpace(authOptions.Scalar.ClientSecret))
                {
                    flow.WithClientSecret(authOptions.Scalar.ClientSecret);
                }
            });

        return options;
    }
}
