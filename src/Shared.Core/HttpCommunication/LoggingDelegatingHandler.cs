using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Shared.Core.HttpCommunication;

public partial class LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        LogRequestStarting(logger, request.Method, request.RequestUri);

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        sw.Stop();

        LogRequestCompleted(logger, request.Method, request.RequestUri, (int)response.StatusCode, sw.ElapsedMilliseconds);

        return response;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "HTTP {Method} {Uri} starting")]
    private static partial void LogRequestStarting(ILogger logger, HttpMethod method, Uri? uri);

    [LoggerMessage(Level = LogLevel.Information, Message = "HTTP {Method} {Uri} -> {StatusCode} in {Elapsed}ms")]
    private static partial void LogRequestCompleted(ILogger logger, HttpMethod method, Uri? uri, int statusCode, long elapsed);
}