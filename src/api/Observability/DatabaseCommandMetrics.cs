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
        var tags = CreateCommandTags(command, dbContext, executeMethod, commandSource);

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
        var commandTags = CreateCommandTags(command, dbContext, executeMethod, commandSource);
        var failureTags = CreateFailureTags(commandTags, exception);

        DatabaseCommands.Add(1, commandTags);
        DatabaseCommandDuration.Record(duration.TotalSeconds, commandTags);
        DatabaseCommandFailures.Add(1, failureTags);
    }

    private static KeyValuePair<string, object?>[] CreateCommandTags(
        DbCommand command,
        DbContext? dbContext,
        DbCommandMethod executeMethod,
        CommandSource commandSource)
    {
        return new[]
        {
            new KeyValuePair<string, object?>("db.system", ResolveDatabaseSystem(command)),
            new KeyValuePair<string, object?>("db.name", ResolveDatabaseName(command)),
            new KeyValuePair<string, object?>("db.operation", ResolveOperation(command.CommandText)),
            new KeyValuePair<string, object?>("db.command.type", ResolveCommandType(executeMethod)),
            new KeyValuePair<string, object?>("db.command.source", commandSource.ToString()),
            new KeyValuePair<string, object?>("db.context", dbContext?.GetType().Name ?? "unknown")
        };
    }

    private static KeyValuePair<string, object?>[] CreateFailureTags(
        KeyValuePair<string, object?>[] commandTags,
        Exception exception)
    {
        var failureTags = new KeyValuePair<string, object?>[commandTags.Length + 1];
        Array.Copy(commandTags, failureTags, commandTags.Length);
        failureTags[^1] = new KeyValuePair<string, object?>("error_type", exception.GetType().Name);

        return failureTags;
    }

    private static string ResolveDatabaseSystem(DbCommand command)
    {
        var connectionTypeName = command.Connection?.GetType().FullName;
        return connectionTypeName is not null &&
               connectionTypeName.StartsWith("Npgsql.", StringComparison.Ordinal)
            ? "postgresql"
            : "unknown";
    }

    private static string ResolveDatabaseName(DbCommand command)
    {
        return string.IsNullOrWhiteSpace(command.Connection?.Database)
            ? "unknown"
            : command.Connection.Database;
    }

    private static string ResolveCommandType(DbCommandMethod executeMethod)
    {
        return executeMethod switch
        {
            DbCommandMethod.ExecuteReader => "reader",
            DbCommandMethod.ExecuteScalar => "scalar",
            DbCommandMethod.ExecuteNonQuery => "non_query",
            _ => executeMethod.ToString()
        };
    }

    private static string ResolveOperation(string commandText)
    {
        var trimmedCommandText = commandText.TrimStart();
        if (trimmedCommandText.Length == 0)
        {
            return "unknown";
        }

        var operationEnd = trimmedCommandText.Length;
        for (var index = 0; index < trimmedCommandText.Length; index++)
        {
            if (char.IsWhiteSpace(trimmedCommandText[index]))
            {
                operationEnd = index;
                break;
            }
        }

        return trimmedCommandText[..operationEnd].ToUpperInvariant();
    }
}
