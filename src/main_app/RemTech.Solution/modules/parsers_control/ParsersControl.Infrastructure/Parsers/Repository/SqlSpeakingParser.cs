using Dapper;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

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

    public Result<SubscribedParser> PermantlyEnable()
    {
        Result<SubscribedParser> result = Inner.PermantlyEnable();
        if (result.IsFailure) return result.Error;
        
        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET state = @state, started_at = @started_at
                           WHERE id = @id
                           """;
        
        object parameters = ExtractParserParameters(result.Value);
        EnqueueChangeRequest(sql, parameters);
        Inner = result.Value;
        return result;
    }

    public SubscribedParser PermantlyDisable()
    {
        SubscribedParser result = Inner.PermantlyDisable();
        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET state = @state
                           WHERE id = @id
                           """;
        object parameters = ExtractParserParameters(result);
        EnqueueChangeRequest(sql, parameters);
        Inner = result;
        return result;
    }

    public Result<SubscribedParserLink> RemoveLink(SubscribedParserLink link)
    {
        Result<SubscribedParserLink> result = Inner.RemoveLink(link);
        if (result.IsFailure) return result.Error;
        
        const string sql = """
                           DELETE FROM parsers_control_module.parser_links
                           WHERE id = @id
                           """;
        
        object parameters = ExtractLinkParameters(result.Value);
        EnqueueChangeRequest(sql, parameters);
        return result;
    }

    public Result<SubscribedParserLink> AddLinkParsedAmount(SubscribedParserLink link, int count)
    {
        Result<SubscribedParserLink> result = Inner.AddLinkParsedAmount(link, count);
        if (result.IsFailure) return result.Error;
        
        const string sql = """
                           UPDATE parsers_control_module.parser_links
                           SET processed = @processed
                           WHERE id = @id
                           """;
        
        object parameters = ExtractLinkParameters(result.Value);
        EnqueueChangeRequest(sql, parameters);
        return result;
    }

    public Result<SubscribedParserLink> AddLinkWorkTime(SubscribedParserLink link, long totalElapsedSeconds)
    {
        Result<SubscribedParserLink> result = Inner.AddLinkWorkTime(link, totalElapsedSeconds);
        if (result.IsFailure) return result.Error;
        
        const string sql = """
                           UPDATE parsers_control_module.parser_links
                           SET elapsed_seconds = @elapsed_seconds
                           WHERE id = @id
                           """;
        
        object parameters = ExtractLinkParameters(result.Value);
        EnqueueChangeRequest(sql, parameters);
        return result;  
    }

    public Result<SubscribedParserLink> EditLink(SubscribedParserLink link, string? newName, string? newUrl)
    {
        Result<SubscribedParserLink> result = Inner.EditLink(link, newName, newUrl);
        if (result.IsFailure) return result.Error;
        
        const string sql = """
                           UPDATE parsers_control_module.parser_links
                           SET name = @name, url = @url
                           WHERE id = @id
                           """;
        
        object parameters = ExtractLinkParameters(result.Value);
        EnqueueChangeRequest(sql, parameters);
        return result;
    }

    public Result<SubscribedParser> Enable()
    {
        Result<SubscribedParser> result = Inner.Enable();
        if (result.IsFailure) return result.Error;
        
        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET state = @state, started_at = @started_at
                           WHERE id = @id
                           """;
        
        object parameters = ExtractParserParameters(result.Value);
        EnqueueChangeRequest(sql, parameters);
        Inner = result.Value;
        return result;
    }

    public Result<IEnumerable<SubscribedParserLink>> AddLinks(IEnumerable<SubscribedParserLinkUrlInfo> urlInfos)
    {
        Result<IEnumerable<SubscribedParserLink>> result = Inner.AddLinks(urlInfos);
        if (result.IsFailure) return result.Error;
        
        const string sql = """
                           INSERT INTO parsers_control_module.parser_links
                           (id, parser_id, name, url, is_active, processed, elapsed_seconds)
                           VALUES (@id, @parser_id, @name, @url, @is_active, @processed, @elapsed_seconds)
                           """;

        object[] parameters = result.Value.Select(ExtractLinkParameters).ToArray();
        PendingTasks.Enqueue(Session.ExecuteBulk(sql, parameters));
        return result;
    }

    public Result<SubscribedParser> AddParserAmount(int amount)
    {
        Result<SubscribedParser> result = Inner.AddParserAmount(amount);
        if (result.IsFailure) return result.Error;
        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET processed = @processed
                           WHERE id = @id
                           """;
        
        object parameters = ExtractParserParameters(result.Value);
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
        
        object parameters = ExtractParserParameters(result.Value);
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
        
        object parameters = ExtractParserParameters(result);
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
        
        object parameters = ExtractParserParameters(result);
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
        
        object parameters = ExtractParserParameters(result.Value);
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
        
        object parameters = ExtractParserParameters(result);
        EnqueueChangeRequest(sql, parameters);
        Inner = result;
        return result;
    }

    Result<SubscribedParser> ISubscribedParser.FinishWork()
    {
        return FinishWork();
    }

    public Result<SubscribedParser> FinishWork()
    {
        Result<SubscribedParser> result = Inner.FinishWork();
        if (result.IsFailure) return result.Error;
        
        const string sql = """
                           UPDATE parsers_control_module.registered_parsers
                           SET state = @state, finished_at = @finished_at, next_run = @next_run
                           WHERE id = @id
                           """;
        
        object parameters = ExtractParserParameters(result.Value);
        EnqueueChangeRequest(sql, parameters);
        Inner = result.Value;
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
        
        object parameters = ExtractParserParameters(result.Value);
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
        
        object parameters = ExtractParserParameters(result.Value);
        EnqueueChangeRequest(sql, parameters);
        Inner = result.Value;
        return result;
    }

    public Result<SubscribedParserLink> FindLink(Func<SubscribedParserLinkUrlInfo, bool> predicate)
    {
        return Inner.FindLink(predicate);
    }

    public Result<SubscribedParserLink> FindLink(Guid id)
    {
        return Inner.FindLink(id);
    }

    public Result<SubscribedParserLink> FindLink(SubscribedParserLinkId id)
    {
        return Inner.FindLink(id);
    }

    public Result<SubscribedParserLink> ChangeLinkActivity(SubscribedParserLink link, bool isActive)
    {
        Result<SubscribedParserLink> result = Inner.ChangeLinkActivity(link, isActive);
        if (result.IsFailure) return result.Error;
        
        const string sql = """
                           UPDATE parsers_control_module.parser_links
                           SET is_active = @is_active
                           WHERE id = @id
                           """;
        
        object parameters = ExtractLinkParameters(result.Value);
        EnqueueChangeRequest(sql, parameters);
        return result;
    }

    public async Task Save()
    {
        while (PendingTasks.Count > 0)
        {
            await PendingTasks.Dequeue();
        }
    }

    private object ExtractParserParameters(SubscribedParser parser)
    {
        return new
        {
            id = parser.Id.Value,
            type = parser.Identity.ServiceType,
            domain = parser.Identity.DomainName,
            state = parser.State.Value,
            processed = parser.Statistics.ParsedCount.Value,
            elapsed_seconds = parser.Statistics.WorkTime.TotalElapsedSeconds,
            wait_days = parser.Schedule.WaitDays.HasValue 
                ? parser.Schedule.WaitDays!.Value 
                : (object)null!,
            next_run = parser.Schedule.NextRun.HasValue 
                ? parser.Schedule.NextRun!.Value 
                : (object)null!,
            started_at = parser.Schedule.StartedAt.HasValue 
                ? parser.Schedule.StartedAt!.Value 
                : (object)null!,
            finished_at = parser.Schedule.FinishedAt.HasValue 
                ? parser.Schedule.FinishedAt!.Value 
                : (object)null!
        };
    }
    
    private object ExtractLinkParameters(SubscribedParserLink link)
    {
        return new
        {
            id = link.Id.Value,
            parser_id = link.ParserId.Value,
            name = link.UrlInfo.Name,
            url = link.UrlInfo.Url,
            is_active = link.Active,
            processed = link.Statistics.ParsedCount.Value,
            elapsed_seconds = link.Statistics.WorkTime.TotalElapsedSeconds
        };
    }
    
    private void EnqueueChangeRequest(string sql, object parameters)
    {
        CommandDefinition command = CreateCommand(sql, parameters);
        PendingTasks.Enqueue(Session.Execute(command));
    }
    
    private CommandDefinition CreateCommand(string sql, object parameters)
    {
        return new(sql, parameters, cancellationToken: CancellationToken, transaction: Session.Transaction);
    }
}