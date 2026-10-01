namespace Shared.Framework.Cors;

/// <summary>
/// Настройки CORS (секция Cors): origins, credentials, headers и methods.
/// </summary>
public class CorsSettings
{
    public const string SECTION_NAME = "Cors";

    public IReadOnlyList<string> AllowedOrigins { get; init; } = [];

    public bool AllowCredentials { get; init; } = true;

    public IReadOnlyList<string> AllowedHeaders { get; init; } = [];

    public IReadOnlyList<string> AllowedMethods { get; init; } = [];
}