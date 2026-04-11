using Microsoft.AspNetCore.Http.Features;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace api.Observability;

public static class ObservabilityExtensions
{
    private const string HttpServerRequestDurationMetricName = "http.server.request.duration";

    private static readonly double[] HttpServerRequestDurationBuckets =
    [
        0.005,
        0.01,
        0.025,
        0.05,
        0.075,
        0.1,
        0.25,
        0.5,
        0.75,
        1,
        2.5,
        5,
        7.5,
        10,
    ];

    private static readonly double[] DatabaseCommandDurationBuckets =
    [
        0.001,
        0.002,
        0.005,
        0.01,
        0.025,
        0.05,
        0.1,
        0.25,
        0.5,
        1,
        2.5,
        5,
    ];

    public static IServiceCollection AddFenixObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var otlpEndpoint = ResolveOtlpEndpoint(configuration);
        var otlpProtocol = ResolveOtlpProtocol(configuration);

        services.AddSingleton<DatabaseCommandMetricsInterceptor>();

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(FenixMetrics.MeterName))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddMeter(FenixMetrics.MeterName)
                    .AddView(
                        HttpServerRequestDurationMetricName,
                        new ExplicitBucketHistogramConfiguration
                        {
                            Boundaries = HttpServerRequestDurationBuckets,
                        })
                    .AddView(
                        DatabaseCommandMetrics.CommandDurationMetricName,
                        new ExplicitBucketHistogramConfiguration
                        {
                            Boundaries = DatabaseCommandDurationBuckets,
                        })
                    .AddPrometheusExporter(options =>
                    {
                        options.ScrapeEndpointPath = "/metrics";
                        options.ScrapeResponseCacheDurationMilliseconds = 0;
                    });
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = context => context.Request.Path != "/metrics";
                        options.RecordException = true;
                    })
                    .AddSource(FenixTracing.ActivitySourceName);

                if (otlpEndpoint is not null)
                {
                    tracing.AddOtlpExporter(options =>
                    {
                        options.Endpoint = otlpEndpoint;

                        if (otlpProtocol.HasValue)
                        {
                            options.Protocol = otlpProtocol.Value;
                        }
                    });
                }
            });

        return services;
    }

    public static IApplicationBuilder UseFenixMetricsEndpoint(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/metrics")
            {
                var httpMetricsTagsFeature = context.Features.Get<IHttpMetricsTagsFeature>();
                if (httpMetricsTagsFeature is not null)
                {
                    httpMetricsTagsFeature.MetricsDisabled = true;
                }
            }

            await next(context);
        });

        return app.UseOpenTelemetryPrometheusScrapingEndpoint();
    }

    private static Uri? ResolveOtlpEndpoint(IConfiguration configuration)
    {
        var endpoint = configuration["Observability:Tracing:Otlp:Endpoint"];

        return Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri)
            ? endpointUri
            : null;
    }

    private static OtlpExportProtocol? ResolveOtlpProtocol(IConfiguration configuration)
    {
        var protocol = configuration["Observability:Tracing:Otlp:Protocol"];

        if (string.IsNullOrWhiteSpace(protocol))
        {
            return null;
        }

        return protocol.Trim().ToLowerInvariant() switch
        {
            "grpc" => OtlpExportProtocol.Grpc,
            "http/protobuf" => OtlpExportProtocol.HttpProtobuf,
            _ => throw new InvalidOperationException(
                $"Unsupported OTLP protocol '{protocol}'. Use 'grpc' or 'http/protobuf'.")
        };
    }
}
