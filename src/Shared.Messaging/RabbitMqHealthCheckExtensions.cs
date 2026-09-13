using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Shared.Messaging;

public static class RabbitMqHealthCheckExtensions
{
    public static IHealthChecksBuilder AddRabbitMqCheck(
        this IHealthChecksBuilder builder,
        IConfiguration configuration,
        string name = "rabbitmq")
    {
        string? connectionString = configuration.GetConnectionString("RabbitMq");

        if (string.IsNullOrWhiteSpace(connectionString) || !Uri.TryCreate(connectionString, UriKind.Absolute, out Uri? uri))
        {
            return builder.AddCheck(name, () =>
                HealthCheckResult.Unhealthy("ConnectionStrings:RabbitMq is not configured"));
        }

        return builder.AddCheck(name, new RabbitMqHealthCheck(uri));
    }
}