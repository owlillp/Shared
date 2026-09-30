namespace Shared.Framework.Authentication;

public sealed class BearerTokenSettings
{
    public const string SECTION_NAME = "Jwt";

    public string Authority { get; set; } = string.Empty;

    public IReadOnlyList<string> Audiences { get; set; } = [];
}