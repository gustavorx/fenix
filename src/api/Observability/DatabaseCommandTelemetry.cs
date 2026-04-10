using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace api.Observability;

internal static class DatabaseCommandTelemetry
{
    public static DatabaseCommandMetadata CreateMetadata(
        DbCommand command,
        DbContext? dbContext,
        DbCommandMethod executeMethod,
        CommandSource commandSource)
    {
        return new DatabaseCommandMetadata(
            ResolveDatabaseSystem(command),
            ResolveDatabaseName(command),
            ResolveOperation(command.CommandText),
            ResolveCommandType(executeMethod),
            commandSource.ToString(),
            dbContext?.GetType().Name ?? "unknown");
    }

    public static KeyValuePair<string, object?>[] CreateCommandTags(DatabaseCommandMetadata metadata)
    {
        return new[]
        {
            new KeyValuePair<string, object?>("db.system", metadata.DatabaseSystem),
            new KeyValuePair<string, object?>("db.name", metadata.DatabaseName),
            new KeyValuePair<string, object?>("db.operation", metadata.Operation),
            new KeyValuePair<string, object?>("db.command.type", metadata.CommandType),
            new KeyValuePair<string, object?>("db.command.source", metadata.CommandSource),
            new KeyValuePair<string, object?>("db.context", metadata.DbContextName)
        };
    }

    public static KeyValuePair<string, object?>[] CreateFailureTags(
        DatabaseCommandMetadata metadata,
        Exception exception)
    {
        var commandTags = CreateCommandTags(metadata);
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

internal sealed record DatabaseCommandMetadata(
    string DatabaseSystem,
    string DatabaseName,
    string Operation,
    string CommandType,
    string CommandSource,
    string DbContextName)
{
    public string ActivityName => $"{Operation} {DatabaseName}";
}
