using System.Text.Json.Serialization;

namespace Shared.SharedKernel.Errors;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorBehavior
{
    Permanent,
    Transient,
}