using Microsoft.AspNetCore.Http.Features;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace api.Observability;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddFenixObservability(this IServiceCollection services)
    {
        services.AddSingleton<DatabaseCommandMetricsInterceptor>();

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(FenixMetrics.MeterName))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddMeter(FenixMetrics.MeterName)
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
}
