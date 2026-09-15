using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace Shared.Framework.Logging;

public static class LoggingExtensions
{
    public static ILogger CreateBootstrapLogger(string serviceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("service_name", serviceName)
            .Enrich.WithProperty(
                "environment",
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Production)
            .Enrich.WithProperty("logger_phase", "bootstrap")
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .CreateBootstrapLogger();
    }

    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder, string serviceName)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        string environmentName = builder.Environment.EnvironmentName;

        builder.Configuration
            .AddJsonFile("serilog.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"serilog.{environmentName}.json", optional: true, reloadOnChange: true);

        builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("service_name", serviceName)
            .Enrich.WithProperty("environment", context.HostingEnvironment.EnvironmentName));

        return builder;
    }

    public static IApplicationBuilder UseSerilogHttpRequestLogging(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.GetLevel = (httpContext, _, exception) =>
            {
                if (exception is not null || httpContext.Response.StatusCode >= StatusCodes.Status500InternalServerError)
                {
                    return LogEventLevel.Error;
                }

                return LogEventLevel.Information;
            };
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("request_id", httpContext.TraceIdentifier);
                diagnosticContext.Set("request_method", httpContext.Request.Method);
                diagnosticContext.Set("request_path", httpContext.Request.Path.Value ?? string.Empty);
                diagnosticContext.Set("request_scheme", httpContext.Request.Scheme);
                diagnosticContext.Set("request_host", httpContext.Request.Host.Value);
                diagnosticContext.Set("response_status_code", httpContext.Response.StatusCode);

                string userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                if (!string.IsNullOrWhiteSpace(userAgent))
                {
                    diagnosticContext.Set("user_agent", userAgent);
                }
            };
        });
    }
}