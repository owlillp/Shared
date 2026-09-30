namespace Shared.Framework.PlatformBootstrap;

public sealed class PlatformOptions
{
    public static readonly string SECTION_NAME = "PlatformSettings";

    public string? ScalarTitle { get; set; }

    public bool EnableJwtAuthentication { get; set; } = true;

    public bool EnableOpenApi { get; set; } = true;

    public bool EnableObservability { get; set; } = true;

    public bool EnableCors { get; set; } = true;

    public IReadOnlyList<string> TrustedProxyNetworks { get; init; } =
    [
        "172.16.0.0/12",
        "127.0.0.0/8",
    ];
}