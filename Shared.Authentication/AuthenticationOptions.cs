namespace Shared.Authentication;

/// <summary>
/// Настройки аутентификации из секции <c>Authentication</c>: адреса OIDC-провайдера и параметры Scalar.
/// </summary>
public sealed class AuthenticationOptions
{
    public const string SECTION_NAME = "Authentication";

    /// <summary>Адрес authorization endpoint OIDC-провайдера (например, OpenIddict <c>/connect/authorize</c>).</summary>
    public string? AuthorizationUrl { get; init; }

    /// <summary>Адрес token endpoint OIDC-провайдера (например, OpenIddict <c>/connect/token</c>).</summary>
    public string? TokenUrl { get; init; }

    /// <summary>Audience по умолчанию; используется как fallback для <see cref="ScalarAuthOptions.ClientId"/>.</summary>
    public string? Audience { get; init; }

    /// <summary>Параметры OAuth2-клиента для Scalar UI (секция <c>Authentication:Scalar</c>).</summary>
    public ScalarAuthOptions Scalar { get; init; } = new();
}

/// <summary>
/// Параметры OAuth2-клиента, подключаемого в Scalar UI.
/// </summary>
public sealed class ScalarAuthOptions
{
    /// <summary>Идентификатор клиента, зарегистрированного в OIDC-провайдере.</summary>
    public string? ClientId { get; init; }

    /// <summary>Секрет клиента; для публичного клиента с PKCE не требуется.</summary>
    public string? ClientSecret { get; init; }
}
