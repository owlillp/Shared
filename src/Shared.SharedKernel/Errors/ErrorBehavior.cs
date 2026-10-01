using System.Text.Json.Serialization;

namespace Shared.SharedKernel.Errors;

[JsonConverter(typeof(JsonStringEnumConverter))]
/// <summary>
/// Поведение ошибки при обработке: permanent или transient (допускает повторные попытки).
/// </summary>
public enum ErrorBehavior
{
    Permanent,
    Transient,
}