namespace Shared.Authentication.Authentication.HttpServerClients;

/// <summary>
/// Настройки получения сервисного токена по client_credentials (секция ServiceToken).
/// </summary>
public sealed class ServiceTokenOptions
{
    public const string SECTION_NAME = "ServiceToken";

    public string TokenUrl { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public IReadOnlyList<string> Scopes { get; set; } = [];

    public bool IsConfigured =>
        !string.IsNullOrEmpty(TokenUrl)
        && !string.IsNullOrEmpty(ClientId)
        && !string.IsNullOrEmpty(ClientSecret);
}
