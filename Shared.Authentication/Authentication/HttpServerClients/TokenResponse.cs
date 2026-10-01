using System.Text.Json.Serialization;

namespace Shared.Authentication.Authentication.HttpServerClients;

/// <summary>
/// Модель ответа token endpoint (access_token, expires_in, token_type).
/// </summary>
public sealed record TokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("token_type")] string TokenType);