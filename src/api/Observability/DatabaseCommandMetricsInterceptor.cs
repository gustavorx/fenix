using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace api.Observability;

public sealed class DatabaseCommandMetricsInterceptor : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        RecordCommandSucceeded(command, eventData);

        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        RecordCommandSucceeded(command, eventData);

        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override int NonQueryExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result)
    {
        RecordCommandSucceeded(command, eventData);

        return base.NonQueryExecuted(command, eventData, result);
    }

    public override ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        RecordCommandSucceeded(command, eventData);

        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override object? ScalarExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result)
    {
        RecordCommandSucceeded(command, eventData);

        return base.ScalarExecuted(command, eventData, result);
    }

    public override ValueTask<object?> ScalarExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result,
        CancellationToken cancellationToken = default)
    {
        RecordCommandSucceeded(command, eventData);

        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override void CommandFailed(
        DbCommand command,
        CommandErrorEventData eventData)
    {
        RecordCommandFailed(command, eventData);

        base.CommandFailed(command, eventData);
    }

    public override Task CommandFailedAsync(
        DbCommand command,
        CommandErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        RecordCommandFailed(command, eventData);

        return base.CommandFailedAsync(command, eventData, cancellationToken);
    }

    private static void RecordCommandSucceeded(
        DbCommand command,
        CommandExecutedEventData eventData)
    {
        DatabaseCommandMetrics.RecordCommandSucceeded(
            command,
            eventData.Context,
            eventData.ExecuteMethod,
            eventData.CommandSource,
            eventData.Duration);
    }

    private static void RecordCommandFailed(
        DbCommand command,
        CommandErrorEventData eventData)
    {
        DatabaseCommandMetrics.RecordCommandFailed(
            command,
            eventData.Context,
            eventData.ExecuteMethod,
            eventData.CommandSource,
            eventData.Duration,
            eventData.Exception);
    }
}
