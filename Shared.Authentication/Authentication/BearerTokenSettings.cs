namespace Shared.Authentication.Authentication;

/// <summary>
/// Настройки JWT Bearer (секция Jwt): authority и список допустимых аудиторий.
/// </summary>
public sealed class BearerTokenSettings
{
    public const string SECTION_NAME = "Jwt";

    public string Authority { get; set; } = string.Empty;

    public IReadOnlyList<string> Audiences { get; set; } = [];
}