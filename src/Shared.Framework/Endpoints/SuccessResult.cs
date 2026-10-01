using System.Net;
using Microsoft.AspNetCore.Http;
using Shared.SharedKernel;

namespace Shared.Framework.Endpoints;

/// <summary>
/// IResult успешного ответа с пустым Envelope.
/// </summary>
public class SuccessResult : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope.Ok();

        httpContext.Response.StatusCode = (int)HttpStatusCode.OK;

        return httpContext.Response.WriteAsJsonAsync(envelope, CancellationToken.None);
    }
}

/// <summary>
/// IResult успешного ответа с Envelope, содержащим полезную нагрузку.
/// </summary>
public class SuccessResult<TValue>(TValue value) : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope.Ok(value);

        httpContext.Response.StatusCode = StatusCodes.Status200OK;

        return httpContext.Response.WriteAsJsonAsync(envelope, CancellationToken.None);
    }
}