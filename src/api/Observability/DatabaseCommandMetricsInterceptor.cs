using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace api.Observability;

public sealed class DatabaseCommandMetricsInterceptor : DbCommandInterceptor
{
    private readonly ConcurrentDictionary<Guid, Activity> commandActivities = new();

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        StartCommandActivity(command, eventData);

        return base.ReaderExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        StartCommandActivity(command, eventData);

        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        RecordCommandSucceeded(command, eventData);
        StopCommandActivity(eventData.CommandId);

        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        RecordCommandSucceeded(command, eventData);
        StopCommandActivity(eventData.CommandId);

        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        StartCommandActivity(command, eventData);

        return base.NonQueryExecuting(command, eventData, result);
    }

    public override int NonQueryExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result)
    {
        RecordCommandSucceeded(command, eventData);
        StopCommandActivity(eventData.CommandId);

        return base.NonQueryExecuted(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        StartCommandActivity(command, eventData);

        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        RecordCommandSucceeded(command, eventData);
        StopCommandActivity(eventData.CommandId);

        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result)
    {
        StartCommandActivity(command, eventData);

        return base.ScalarExecuting(command, eventData, result);
    }

    public override object? ScalarExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result)
    {
        RecordCommandSucceeded(command, eventData);
        StopCommandActivity(eventData.CommandId);

        return base.ScalarExecuted(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result,
        CancellationToken cancellationToken = default)
    {
        StartCommandActivity(command, eventData);

        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<object?> ScalarExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result,
        CancellationToken cancellationToken = default)
    {
        RecordCommandSucceeded(command, eventData);
        StopCommandActivity(eventData.CommandId);

        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override void CommandCanceled(
        DbCommand command,
        CommandEndEventData eventData)
    {
        StopCommandActivity(eventData.CommandId, errorType: "CommandCanceled", isCanceled: true);

        base.CommandCanceled(command, eventData);
    }

    public override Task CommandCanceledAsync(
        DbCommand command,
        CommandEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        StopCommandActivity(eventData.CommandId, errorType: "CommandCanceled", isCanceled: true);

        return base.CommandCanceledAsync(command, eventData, cancellationToken);
    }

    public override void CommandFailed(
        DbCommand command,
        CommandErrorEventData eventData)
    {
        RecordCommandFailed(command, eventData);
        StopCommandActivity(eventData.CommandId, eventData.Exception.GetType().Name);

        base.CommandFailed(command, eventData);
    }

    public override Task CommandFailedAsync(
        DbCommand command,
        CommandErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        RecordCommandFailed(command, eventData);
        StopCommandActivity(eventData.CommandId, eventData.Exception.GetType().Name);

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

    private void StartCommandActivity(
        DbCommand command,
        CommandEventData eventData)
    {
        var metadata = DatabaseCommandTelemetry.CreateMetadata(
            command,
            eventData.Context,
            eventData.ExecuteMethod,
            eventData.CommandSource);

        var activity = FenixTracing.ActivitySource.StartActivity(metadata.ActivityName, ActivityKind.Client);
        if (activity is null)
        {
            return;
        }

        activity.SetTag("db.system", metadata.DatabaseSystem);
        activity.SetTag("db.name", metadata.DatabaseName);
        activity.SetTag("db.operation", metadata.Operation);
        activity.SetTag("db.command.type", metadata.CommandType);
        activity.SetTag("db.command.source", metadata.CommandSource);
        activity.SetTag("db.context", metadata.DbContextName);

        if (!commandActivities.TryAdd(eventData.CommandId, activity))
        {
            activity.Dispose();
        }
    }

    private void StopCommandActivity(
        Guid commandId,
        string? errorType = null,
        bool isCanceled = false)
    {
        if (!commandActivities.TryRemove(commandId, out var activity))
        {
            return;
        }

        if (errorType is null)
        {
            activity.SetStatus(ActivityStatusCode.Ok);
        }
        else
        {
            activity.SetStatus(ActivityStatusCode.Error, errorType);
            activity.SetTag("error.type", errorType);

            if (isCanceled)
            {
                activity.SetTag("db.command.canceled", true);
            }
        }

        activity.Stop();
        activity.Dispose();
    }
}
