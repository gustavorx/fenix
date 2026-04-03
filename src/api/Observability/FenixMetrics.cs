using System.Diagnostics.Metrics;

namespace api.Observability;

public static class FenixMetrics
{
    public const string MeterName = "Fenix.Api";

    public static readonly Meter Meter = new(MeterName);
}
