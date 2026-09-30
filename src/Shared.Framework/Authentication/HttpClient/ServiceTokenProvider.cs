using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Shared.SharedKernel.Errors;

namespace Shared.Framework.Authentication.HttpClient;

public sealed class ServiceTokenProvider(
    IHttpClientFactory httpClientFactory,
    TimeProvider timeProvider,
    IOptions<ServiceTokenOptions> options) : IDisposable
{
    private static readonly TimeSpan REFRESH_BEFORE_EXPIRY = TimeSpan.FromSeconds(30);

    private readonly ServiceTokenOptions _options = options.Value;

    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _cachedToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

    public async Task<Result<string, Error>> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.TokenUrl) || string.IsNullOrWhiteSpace(_options.ClientId))
        {
            return Error.Failure(
                "service_token.not_configured",
                "Service client_credentials (TokenUrl/ClientId) are not configured");
        }

        if (TryGetCached(out string cached))
            return cached;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (TryGetCached(out cached))
            {
                return cached;
            }
            return await FetchAsync(cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<Result<string, Error>> FetchAsync(CancellationToken cancellationToken)
    {
        var form = new Dictionary<string, string>()
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
        };

        if (!string.IsNullOrWhiteSpace(_options.Scope))
        {
            form["scope"] = _options.Scope;
        }

        using var client = httpClientFactory.CreateClient();
        using var content = new FormUrlEncodedContent(form);
        using var response = await client.PostAsync(_options.TokenUrl, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure(
                "service_token.request_failed",
                $"Token endpoint returned {(int)response.StatusCode}");
        }

        var body = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken);
        if (body is null || string.IsNullOrWhiteSpace(body.AccessToken))
        {
            return Error.Failure(
                "service_token.invalid_response",
                "Token endpoint returned empty access token");
        }

        _cachedToken = body.AccessToken;
        _expiresAt = timeProvider.GetUtcNow().AddSeconds(body.ExpiresIn);
        return body.AccessToken;
    }

    private bool TryGetCached(out string token)
    {
        string? cached = _cachedToken;
        if (cached != null && timeProvider.GetUtcNow() < _expiresAt - REFRESH_BEFORE_EXPIRY)
        {
            token = cached;
            return true;
        }

        token = string.Empty;
        return false;
    }

    public void Dispose() => _lock.Dispose();
}