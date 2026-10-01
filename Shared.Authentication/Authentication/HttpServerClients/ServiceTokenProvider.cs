using System.Net;
using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.SharedKernel.Errors;

namespace Shared.Authentication.Authentication.HttpServerClients;

/// <summary>
/// Кэширующий провайдер access-токена, получаемого по client_credentials.
/// </summary>
public sealed class ServiceTokenProvider(
    IHttpClientFactory httpClientFactory,
    TimeProvider timeProvider,
    ILogger<ServiceTokenProvider> logger,
    IOptions<ServiceTokenOptions> options) : IDisposable
{
    private static readonly TimeSpan _refreshBeforeExpiry = TimeSpan.FromSeconds(30);

    private readonly ServiceTokenOptions _options = options.Value;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private string? _cachedToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

    public void Dispose() => _semaphore.Dispose();

    public async Task<Result<string, Error>> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (!_options.IsConfigured)
        {
            return Error.Failure(
                "service_token.not_configured",
                "Service client_credentials (TokenUrl/ClientId) are not configured");
        }

        if (TryGetCached(out string cached))
        {
            return cached;
        }

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return TryGetCached(out cached)
                ? cached
                : await FetchAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<Result<string, Error>> FetchAsync(CancellationToken cancellationToken)
    {
        try
        {
            using HttpResponseMessage response = await RequestTokenAsync(cancellationToken).ConfigureAwait(false);

            return response.IsSuccessStatusCode
                ? await ReadTokenAsync(response, cancellationToken).ConfigureAwait(false)
                : await HandleFailureAsync(response, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed while requesting service token from {TokenUrl}", _options.TokenUrl);
            return Error.Failure(
                "service.token.unavailable",
                "Token endpoint is unavailable").AsTransient();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Token request timed out for {TokenUrl}", _options.TokenUrl);
            return Error.Failure(
                "service.token.timeout",
                "Token endpoint request timed out").AsTransient();
        }
    }

    private async Task<HttpResponseMessage> RequestTokenAsync(CancellationToken cancellationToken)
    {
        List<KeyValuePair<string, string>> form =
        [
            new ("grant_type", "client_credentials"),
            new ("client_id", _options.ClientId),
            new ("client_secret", _options.ClientSecret),
        ];

        if (_options.Scopes.Count > 0)
        {
            form.Add(new KeyValuePair<string, string>("scope", string.Join(' ', _options.Scopes)));
        }

        using FormUrlEncodedContent requestBody = new(form);

        HttpClient httpClient = httpClientFactory.CreateClient();

        return await httpClient.PostAsync(_options.TokenUrl, requestBody, cancellationToken).ConfigureAwait(false);
    }

    private async Task<Result<string, Error>> ReadTokenAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        TokenResponse? tokenResponse = await response.Content
            .ReadFromJsonAsync<TokenResponse>(cancellationToken)
            .ConfigureAwait(false);

        if (tokenResponse is null || string.IsNullOrEmpty(tokenResponse.AccessToken))
        {
            logger.LogError("Service token response is empty or missing access_token");
            return Error.Failure(
                "service.token.empty_response",
                "Token response is empty or missing access_token");
        }

        if (tokenResponse.ExpiresIn <= 0)
        {
            logger.LogError(
                "Service token response contains invalid expires_in: {ExpiresIn}",
                tokenResponse.ExpiresIn);

            return Error.Failure(
                "service.token.invalid_expiry",
                "Token response contains a non-positive expires_in");
        }

        _cachedToken = tokenResponse.AccessToken;
        _expiresAt = timeProvider.GetUtcNow().AddSeconds(tokenResponse.ExpiresIn);

        logger.LogDebug(
            "Service token obtained, expires in {ExpiresIn}s (cached until {ExpiresAt})",
            tokenResponse.ExpiresIn,
            _expiresAt);

        return _cachedToken;
    }

    private async Task<Result<string, Error>> HandleFailureAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        string errorBody = await response.Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);

        logger.LogError(
            "Failed to obtain service token: {StatusCode} {Error}",
            response.StatusCode,
            errorBody);

        var error = Error.Failure(
            "service.token.request_failed",
            $"Token request failed: {response.StatusCode}");

        return IsTransientStatusCode(response.StatusCode)
            ? error.AsTransient()
            : error;
    }

    private bool TryGetCached(out string token)
    {
        string? cached = _cachedToken;
        if (cached != null && timeProvider.GetUtcNow() < _expiresAt - _refreshBeforeExpiry)
        {
            token = cached;
            return true;
        }

        token = string.Empty;
        return false;
    }

    private static bool IsTransientStatusCode(HttpStatusCode statusCode) =>
        statusCode == HttpStatusCode.RequestTimeout
        || statusCode == HttpStatusCode.TooManyRequests
        || (int)statusCode >= 500;
}
