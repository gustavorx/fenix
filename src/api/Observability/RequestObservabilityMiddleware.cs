using System.Diagnostics;

namespace api.Observability;

public sealed class RequestObservabilityMiddleware(
    RequestDelegate next,
    ILogger<RequestObservabilityMiddleware> logger)
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private const int MaxCorrelationIdLength = 128;

    private static readonly EventId HttpRequestCompleted = new(1000, nameof(HttpRequestCompleted));
    private static readonly EventId HttpRequestFailed = new(1001, nameof(HttpRequestFailed));

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context);
        var requestActivity = Activity.Current;
        var traceId = requestActivity?.TraceId.ToString() ?? context.TraceIdentifier;
        var stopwatch = Stopwatch.StartNew();

        requestActivity?.SetTag("fenix.correlation_id", correlationId);

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["TraceId"] = traceId
        });

        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            HttpServerMetrics.RecordError(
                context,
                StatusCodes.Status500InternalServerError,
                "Unexpected");

            logger.LogError(
                HttpRequestFailed,
                exception,
                "HTTP request failed {Method} {Path} responded {StatusCode} in {DurationMs} ms",
                context.Request.Method,
                context.Request.Path.Value,
                StatusCodes.Status500InternalServerError,
                stopwatch.ElapsedMilliseconds);

            throw;
        }

        stopwatch.Stop();

        var statusCode = context.Response.StatusCode;
        var logLevel = GetLogLevel(statusCode);
        var errorType = GetContextItem<string>(context, ObservabilityHttpContextItemKeys.ErrorType);
        var errorCodes = GetContextItem<IReadOnlyCollection<string>>(context, ObservabilityHttpContextItemKeys.ErrorCodes);
        var errorCodeList = errorCodes is { Count: > 0 }
            ? string.Join(",", errorCodes)
            : null;

        if (statusCode >= StatusCodes.Status400BadRequest)
        {
            HttpServerMetrics.RecordError(
                context,
                statusCode,
                ResolveErrorType(statusCode, errorType));
        }

        if (string.IsNullOrWhiteSpace(errorType))
        {
            logger.Log(
                logLevel,
                HttpRequestCompleted,
                "HTTP request completed {Method} {Path} responded {StatusCode} in {DurationMs} ms",
                context.Request.Method,
                context.Request.Path.Value,
                statusCode,
                stopwatch.ElapsedMilliseconds);

            return;
        }

        logger.Log(
            logLevel,
            HttpRequestCompleted,
            "HTTP request completed {Method} {Path} responded {StatusCode} in {DurationMs} ms with {ErrorType} errors {ErrorCodes}",
            context.Request.Method,
            context.Request.Path.Value,
            statusCode,
            stopwatch.ElapsedMilliseconds,
            errorType,
            errorCodeList);
    }

    private static string ResolveCorrelationId(HttpContext context)
    {
        var requestedCorrelationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(requestedCorrelationId) &&
            requestedCorrelationId.Length <= MaxCorrelationIdLength)
        {
            return requestedCorrelationId.Trim();
        }

        return Guid.NewGuid().ToString("N");
    }

    private static LogLevel GetLogLevel(int statusCode)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            return LogLevel.Error;
        }

        if (statusCode >= StatusCodes.Status400BadRequest)
        {
            return LogLevel.Warning;
        }

        return LogLevel.Information;
    }

    private static string ResolveErrorType(int statusCode, string? errorType)
    {
        if (!string.IsNullOrWhiteSpace(errorType))
        {
            return errorType;
        }

        return statusCode >= StatusCodes.Status500InternalServerError
            ? "Unexpected"
            : "HttpError";
    }

    private static T? GetContextItem<T>(HttpContext context, string key)
    {
        return context.Items.TryGetValue(key, out var value) && value is T typedValue
            ? typedValue
            : default;
    }
}
