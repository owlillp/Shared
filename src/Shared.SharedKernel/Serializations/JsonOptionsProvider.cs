using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.SharedKernel.Serializations;

/// <summary>
/// Общие JsonSerializerOptions (web defaults и строковые значения enum).
/// </summary>
public static class JsonOptionsProvider
{
    public static readonly JsonSerializerOptions Options =
        new(JsonSerializerDefaults.Web)
        {
            Converters =
            {
                new JsonStringEnumConverter(),
            },
        };
}