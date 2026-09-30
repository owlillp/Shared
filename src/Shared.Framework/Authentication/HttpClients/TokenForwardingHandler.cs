using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Shared.Framework.Authentication.HttpClients;

public sealed class TokenForwardingHandler(
    IHttpContextAccessor httpContextAccessor,
    ServiceTokenProvider serviceTokenProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        string? authHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

        if (!string.IsNullOrWhiteSpace(authHeader))
        {
            request.Headers.TryAddWithoutValidation("Authorization", authHeader);
        }
        else
        {
            var token = await serviceTokenProvider.GetTokenAsync(cancellationToken).ConfigureAwait(false);
            if (token.IsSuccess)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            }
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
