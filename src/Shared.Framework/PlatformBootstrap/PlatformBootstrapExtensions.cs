using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using Shared.Framework.Authentication;
using Shared.Framework.Authorization;
using Shared.Framework.Cors;
using Shared.Framework.Logging;
using Shared.Framework.Middlewares;

namespace Shared.Framework.PlatformBootstrap;

public static class PlatformBootstrapExtensions
{
    public static WebApplicationBuilder AddPlatformDefaults(
        this WebApplicationBuilder builder,
        string serviceName)
    {
        builder.Services.Configure<PlatformOptions>(builder.Configuration.GetSection(PlatformOptions.SECTION_NAME));

        PlatformOptions platformOptions = builder.Configuration
            .GetSection(PlatformOptions.SECTION_NAME)
            .Get<PlatformOptions>() ?? new PlatformOptions();

        if (platformOptions.EnableObservability)
        {
            builder.AddSerilogLogging(serviceName);
        }

        if (platformOptions.EnableCors)
        {
            builder.Services.AddFrameworkCors(builder.Configuration);
        }

        if (platformOptions.EnableJwtAuthentication)
        {
            builder.Services.AddBearerAuthentication(builder.Configuration);
        }

        builder.Services.AddRateLimiter(_ => { });
        builder.Services.AddSingleton(new PlatformServiceDescriptor(serviceName));

        return builder;
    }

    public static WebApplication UsePlatformDefaults(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<PlatformOptions>();
        var descriptor = app.Services.GetRequiredService<PlatformServiceDescriptor>();

        var forwardedOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            ForwardLimit = 1,
        };
        forwardedOptions.KnownIPNetworks.Clear();
        forwardedOptions.KnownProxies.Clear();
        foreach (string cidr in options.TrustedProxyNetworks)
        {
            string[] parts = cidr.Split('/', 2);
            if (parts.Length == 2
                && IPAddress.TryParse(parts[0], out IPAddress? addr)
                && int.TryParse(parts[1], System.Globalization.CultureInfo.InvariantCulture, out int prefix))
            {
                forwardedOptions.KnownIPNetworks.Add(new System.Net.IPNetwork(addr, prefix));
            }
        }
        app.UseForwardedHeaders(forwardedOptions);

        if (options.EnableCors)
        {
            app.ConfigureCors();
        }

        app.UseExceptionMiddleware();
        app.UseRequestCorrelationId();

        if (options.EnableJwtAuthentication)
        {
            app.UseBearerAuthentication();
        }

        app.UseRateLimiter();

        if (options.EnableObservability)
        {
            app.UseSerilogHttpRequestLogging();
        }

        if (options.EnableOpenApi && !app.Environment.IsProduction())
        {
            app.MapOpenApi().AllowAnonymousEndpoint();
            app.MapScalarApiReference(scalar =>
            {
                scalar
                    .WithTitle(options.ScalarTitle ?? descriptor.Name)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            }).AllowAnonymousEndpoint();
        }

        return app;
    }
}

internal sealed record PlatformServiceDescriptor(string Name);