using System.Data.Common;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace api.Observability;

public static class DatabaseCommandMetrics
{
    private static readonly Counter<long> DatabaseCommands = FenixMetrics.Meter.CreateCounter<long>(
        "fenix.db.commands",
        "{command}",
        "Database commands observed.");

    private static readonly Histogram<double> DatabaseCommandDuration = FenixMetrics.Meter.CreateHistogram<double>(
        "fenix.db.command.duration",
        "s",
        "Database command duration.");

    private static readonly Counter<long> DatabaseCommandFailures = FenixMetrics.Meter.CreateCounter<long>(
        "fenix.db.command.failures",
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
