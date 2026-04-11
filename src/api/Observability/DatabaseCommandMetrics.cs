using System.Data.Common;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace api.Observability;

public static class DatabaseCommandMetrics
{
    public const string CommandsMetricName = "fenix.db.commands";
    public const string CommandDurationMetricName = "fenix.db.command.duration";
    public const string CommandFailuresMetricName = "fenix.db.command.failures";

    private static readonly Counter<long> DatabaseCommands = FenixMetrics.Meter.CreateCounter<long>(
        CommandsMetricName,
        "{command}",
        "Database commands observed.");

    private static readonly Histogram<double> DatabaseCommandDuration = FenixMetrics.Meter.CreateHistogram<double>(
        CommandDurationMetricName,
        "s",
        "Database command duration.");

    private static readonly Counter<long> DatabaseCommandFailures = FenixMetrics.Meter.CreateCounter<long>(
        CommandFailuresMetricName,
        "{error}",
        "Database command failures.");

    public static void RecordCommandSucceeded(
        DbCommand command,
        DbContext? dbContext,
        DbCommandMethod executeMethod,
        CommandSource commandSource,
        TimeSpan duration)
    {
        var metadata = DatabaseCommandTelemetry.CreateMetadata(command, dbContext, executeMethod, commandSource);
        var tags = DatabaseCommandTelemetry.CreateCommandTags(metadata);

        DatabaseCommands.Add(1, tags);
        DatabaseCommandDuration.Record(duration.TotalSeconds, tags);
    }

    public static void RecordCommandFailed(
        DbCommand command,
        DbContext? dbContext,
        DbCommandMethod executeMethod,
        CommandSource commandSource,
        TimeSpan duration,
        Exception exception)
    {
        var metadata = DatabaseCommandTelemetry.CreateMetadata(command, dbContext, executeMethod, commandSource);
        var commandTags = DatabaseCommandTelemetry.CreateCommandTags(metadata);
        var failureTags = DatabaseCommandTelemetry.CreateFailureTags(metadata, exception);

        DatabaseCommands.Add(1, commandTags);
        DatabaseCommandDuration.Record(duration.TotalSeconds, commandTags);
        DatabaseCommandFailures.Add(1, failureTags);
    }
}
