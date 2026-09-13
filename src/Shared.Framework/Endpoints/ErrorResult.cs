using Microsoft.AspNetCore.Http;
using Shared.SharedKernel;
using Shared.SharedKernel.Errors;

namespace Shared.Framework.Endpoints;

public class ErrorResult(Error error) : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        int statusCode = GetStatusCodeFromErrorType(error.Type);

        var envelope = Envelope.Fail(error);
        httpContext.Response.StatusCode = statusCode;

        return httpContext.Response.WriteAsJsonAsync(envelope, cancellationToken: httpContext.RequestAborted);
    }

    private static int GetStatusCodeFromErrorType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.VALIDATION => StatusCodes.Status400BadRequest,
            ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
            ErrorType.CONFLICT => StatusCodes.Status409Conflict,
            ErrorType.FAILURE => StatusCodes.Status500InternalServerError,
            ErrorType.AUTHENTICATION => StatusCodes.Status401Unauthorized,
            ErrorType.AUTHORIZATION => StatusCodes.Status403Forbidden,
            ErrorType.RATE_LIMIT => StatusCodes.Status429TooManyRequests,
            _ => StatusCodes.Status500InternalServerError
        };
}