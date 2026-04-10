using System.Diagnostics;

namespace api.Observability;

public static class FenixTracing
{
    public const string ActivitySourceName = FenixMetrics.MeterName;

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}
