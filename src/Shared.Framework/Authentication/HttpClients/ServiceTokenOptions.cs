namespace Shared.Framework.Authentication.HttpClients;

public sealed class ServiceTokenOptions
{
    public const string SECTION_NAME = "ServiceToken";

    public string TokenUrl { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    // Пусто = scope не отправляется (сервер выдаст токен без scopes).
    // Задавайте явно в конфиге: ["auth"] и т.п.
    public IReadOnlyList<string> Scopes { get; set; } = [];

    public bool IsConfigured =>
        !string.IsNullOrEmpty(TokenUrl)
        && !string.IsNullOrEmpty(ClientId)
        && !string.IsNullOrEmpty(ClientSecret);
}
