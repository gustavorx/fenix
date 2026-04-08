using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Routing;

namespace api.Observability;

public static class HttpServerMetrics
{
    private static readonly Counter<long> HttpServerErrors = FenixMetrics.Meter.CreateCounter<long>(
        "fenix.http.server.errors",
        "{error}",
        "HTTP error responses.");

    public static void RecordError(HttpContext context, int statusCode, string errorType)
    {
        HttpServerErrors.Add(
            1,
            new KeyValuePair<string, object?>("http.request.method", context.Request.Method),
            new KeyValuePair<string, object?>("http.route", ResolveRoute(context)),
            new KeyValuePair<string, object?>("http.response.status_code", statusCode),
            new KeyValuePair<string, object?>("error_type", errorType));
    }

    private static string ResolveRoute(HttpContext context)
    {
        if (context.GetEndpoint() is RouteEndpoint endpoint &&
            !string.IsNullOrWhiteSpace(endpoint.RoutePattern.RawText))
        {
            return "/" + endpoint.RoutePattern.RawText.TrimStart('/');
        }

        return "unmatched";
    }
}
