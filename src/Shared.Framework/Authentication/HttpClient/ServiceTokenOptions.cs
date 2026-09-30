namespace Shared.Framework.Authentication.HttpClient;

public sealed class ServiceTokenOptions
{
    public const string SECTION_NAME = "ServiceToken";

    public string Subject { get; set; } = "service";

    public string Name { get; set; } = "service";

    public int LifetimeMinutes { get; set; } = 5;

    public string Scope { get; set; } = string.Empty;

    public string TokenUrl { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;
}