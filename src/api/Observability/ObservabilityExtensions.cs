using Microsoft.AspNetCore.Http.Features;
using OpenTelemetry.Metrics;

namespace api.Observability;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddFenixObservability(this IServiceCollection services)
    {
        services.AddSingleton<DatabaseCommandMetricsInterceptor>();

        services
            .AddOpenTelemetry()
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
