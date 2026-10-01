using System.Text.Json.Serialization;

namespace Shared.SharedKernel.Errors;

[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Категория ошибки, определяющая HTTP-статус ответа.
/// </summary>
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