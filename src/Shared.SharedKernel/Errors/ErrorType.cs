using System.Text.Json.Serialization;

namespace Shared.SharedKernel.Errors;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorType
{
    VALIDATION,
    NOT_FOUND,
    FAILURE,
    CONFLICT,
    AUTHENTICATION,
    AUTHORIZATION,
    RATE_LIMIT,
}