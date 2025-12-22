using Dapper;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace ParsersControl.Infrastructure.Parsers.Repository;

public sealed class SqlSpeakingParser(
    NpgSqlSession session, 
    ISubscribedParser inner, 
    CancellationToken ct = default) 
    : ISubscribedParser
{
    private Queue<Task> PendingTasks { get; } = [];
    private ISubscribedParser Inner { get; set; } = inner;
    private CancellationToken CancellationToken { get; } = ct;
    private NpgSqlSession Session { get; } = session;
    
    public Result<SubscribedParser> AddParserAmount(int amount)
    {
        Result<SubscribedParser> result = Inner.AddParserAmount(amount);
        if (result.IsFailure) return result.Error;
        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET processed = @processed
                           WHERE id = @id
                           """;
        
        object parameters = new
        {
            processed = result.Value.Statistics.ParsedCount.Value, 
            id = result.Value.Id.Value
        };
        
        EnqueueChangeRequest(sql, parameters);
        Inner = result.Value;
        return result;
    }

    public Result<SubscribedParser> AddWorkTime(long totalElapsedSeconds)
    {
        Result<SubscribedParser> result = Inner.AddWorkTime(totalElapsedSeconds);
        if (result.IsFailure) return result.Error;
        
        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET elapsed_seconds = @elapsed_seconds
                           WHERE id = @id
                           """;
        
        object parameters = new
        {
            elapsed_seconds = totalElapsedSeconds, 
            id = result.Value.Id.Value
        };
        
        EnqueueChangeRequest(sql, parameters);
        Inner = result.Value;
        return result.Value;
    }

    public SubscribedParser ResetWorkTime()
    {
        SubscribedParser result = Inner.ResetWorkTime();
        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET elapsed_seconds = @elapsed_seconds
                           WHERE id = @id
                           """;
        
        object parameters = new
        {
            elapsed_seconds = result.Statistics.WorkTime.TotalElapsedSeconds, 
            id = result.Id.Value
        };
        
        EnqueueChangeRequest(sql, parameters);
        Inner = result;
        return result;
    }

    public SubscribedParser ResetParsedCount()
    {
        SubscribedParser result = Inner.ResetParsedCount();
        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET processed = @processed
                           WHERE id = @id
                           """;
        
        object parameters = new
        {
            processed = result.Statistics.ParsedCount.Value, 
            id = result.Id.Value
        };
        
        EnqueueChangeRequest(sql, parameters);
        Inner = result;
        return result;
    }

    public Result<SubscribedParser> StartWork()
    {
        Result<SubscribedParser> result = Inner.StartWork();
        if (result.IsFailure) return result.Error;
        
        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET state = @state, started_at = @started_at
                           WHERE id = @id
                           """;
        
        object parameters = new
        {
            state = result.Value.State.Value,
            started_at = result.Value.Schedule.StartedAt!.Value,
            id = result.Value.Id.Value
        };
        
        EnqueueChangeRequest(sql, parameters);
        Inner = result.Value;
        return result;
    }

    public SubscribedParser Disable()
    {
        SubscribedParser result = Inner.Disable();
        
        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET state = @state, finished_at = @finished_at
                           WHERE id = @id
                           """;
        
        object parameters = new
        {
            state = result.State.Value,
            finished_at = result.Schedule.FinishedAt!.Value,
            id = result.Id.Value
        };
        
        EnqueueChangeRequest(sql, parameters);
        Inner = result;
        return result;
    }

    Result<SubscribedParser> ISubscribedParser.FinishWork()
    {
        return FinishWork();
    }

    public SubscribedParser FinishWork()
    {
        SubscribedParser result = Inner.FinishWork();
        
        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET state = @state, finished_at = @finished_at, next_run = @next_run
                           WHERE id = @id
                           """;
        
        object parameters = new
        {
            state = result.State.Value,
            finished_at = result.Schedule.FinishedAt!.Value,
            next_run = result.Schedule.NextRun!.Value,
            id = result.Id.Value
        };
        
        EnqueueChangeRequest(sql, parameters);
        Inner = result;
        return result;
    }

    public Result<SubscribedParser> ChangeScheduleWaitDays(int waitDays)
    {
        Result<SubscribedParser> result = Inner.ChangeScheduleWaitDays(waitDays);
        if (result.IsFailure) return result.Error;

        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET wait_days = @wait_days, next_run = @next_run
                           WHERE id = @id
                           """;
        
        object parameters = new
        {
            wait_days = waitDays, 
            next_run = result.Value.Schedule.NextRun!.Value, 
            id = result.Value.Id.Value
        };
        
        EnqueueChangeRequest(sql, parameters);
        Inner = result.Value;
        return result;
    }

    public Result<SubscribedParser> ChangeScheduleNextRun(DateTime nextRun)
    {
        Result<SubscribedParser> result = Inner.ChangeScheduleNextRun(nextRun);
        if (result.IsFailure) return result.Error;
        
        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET next_run = @next_run
                           WHERE id = @id
                           """;
        
        object parameters = new
        {
            next_run = nextRun,
            id = result.Value.Id.Value
        };
        
        EnqueueChangeRequest(sql, parameters);
        Inner = result.Value;
        return result;
    }

    public async Task Save()
    {
        while (PendingTasks.Count > 0)
        {
            await PendingTasks.Dequeue();
        }
    }
    
    private void EnqueueChangeRequest(string sql, object parameters)
    {
        CommandDefinition command = CreateCommand(sql, parameters);
        PendingTasks.Enqueue(session.Execute(command));
    }
    
    private CommandDefinition CreateCommand(string sql, object parameters)
    {
        return new(sql, parameters, cancellationToken: CancellationToken, transaction: Session.Transaction);
    }
}