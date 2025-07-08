using Dapper;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers;

public sealed class PgTransactionalParser : ITransactionalParser
{
    private readonly IParser _parser;
    private readonly PgTransactionalParserJournal _journal;
    private readonly CancellationToken _ct;

    public PgTransactionalParser(
        IParser parser,
        PgTransactionalParserJournal journal,
        CancellationToken ct = default
    )
    {
        _parser = parser;
        _journal = journal;
        _ct = ct;
    }

    public ParserIdentity Identification() => _parser.Identification();

    public ParserStatistic WorkedStatistics() => _parser.WorkedStatistics();

    public ParserSchedule WorkSchedule() => _parser.WorkSchedule();

    public ParserState WorkState() => _parser.WorkState();

    public ParserLinksBag OwnedLinks() => _parser.OwnedLinks();

    public ParserServiceDomain Domain() => _parser.Domain();

    public Status<ParserStatisticsIncreasement> IncreaseProcessed(IParserLink link)
    {
        Status<ParserStatisticsIncreasement> status = _parser.IncreaseProcessed(link);
        if (status.IsFailure)
            return status;
        string parserProcessedSql = string.Intern(
            "UPDATE parsers_management_module.parsers SET processed = @parser_processed WHERE id = @parser_id"
        );
        var parserProcessedParameters = new
        {
            @parser_processed = status.Value.CurrentProcessed(),
            @parser_id = status.Value.IdOfIncreased(),
        };
        _journal.AddOperation(
            new CommandDefinition(
                parserProcessedSql,
                parserProcessedParameters,
                transaction: _journal.Transaction(),
                cancellationToken: _ct
            )
        );
        string linksSql = string.Intern(
            "UPDATE parsers_management_module.parser_links SET processed = @link_processed WHERE id = @link_id AND parser_id = @parser_link_id"
        );
        var linkSqlParameters = new
        {
            @link_processed = status.Value.LinkIncreasement().CurrentProcessed(),
            @link_id = status.Value.LinkIncreasement().IdOfIncreased(),
            @parser_link_id = status.Value.IdOfIncreased(),
        };
        _journal.AddOperation(
            new CommandDefinition(
                linksSql,
                linkSqlParameters,
                transaction: _journal.Transaction(),
                cancellationToken: _ct
            )
        );
        return status;
    }

    public Status ChangeState(NotEmptyString stateString)
    {
        Status status = _parser.ChangeState(stateString);
        if (status.IsFailure)
            return status;
        string sql = string.Intern(
            "UPDATE parsers_management_module.parsers SET state = @state where id = @id"
        );
        var parameters = new
        {
            state = WorkState().Read().StringValue(),
            id = (Guid)Identification().ReadId(),
        };
        _journal.AddOperation(
            new CommandDefinition(
                sql,
                parameters,
                transaction: _journal.Transaction(),
                cancellationToken: _ct
            )
        );
        return status;
    }

    public Status Enable()
    {
        Status status = _parser.Enable();
        if (status.IsFailure)
            return status;
        string sql = string.Intern(
            "UPDATE parsers_management_module.parsers SET state = @state where id = @id"
        );
        var parameters = new
        {
            state = WorkState().Read().StringValue(),
            id = (Guid)Identification().ReadId(),
        };
        _journal.AddOperation(
            new CommandDefinition(
                sql,
                parameters,
                transaction: _journal.Transaction(),
                cancellationToken: _ct
            )
        );
        return status;
    }

    public Status Disable()
    {
        Status status = _parser.Disable();
        if (status.IsFailure)
            return status;
        string sql = string.Intern(
            "UPDATE parsers_management_module.parsers SET state = @state where id = @id"
        );
        var parameters = new
        {
            state = WorkState().Read().StringValue(),
            id = (Guid)Identification().ReadId(),
        };
        _journal.AddOperation(
            new CommandDefinition(
                sql,
                parameters,
                transaction: _journal.Transaction(),
                cancellationToken: _ct
            )
        );
        return status;
    }

    public Status ChangeWaitDays(PositiveInteger waitDays)
    {
        Status status = _parser.ChangeWaitDays(waitDays);
        if (status.IsFailure)
            return status;
        string sql = string.Intern(
            "UPDATE parsers_management_module.parsers SET wait_days = @wait_days, next_run = @next_run where id = @id"
        );
        var parameters = new
        {
            wait_days = WorkSchedule().WaitDays().Read(),
            next_run = WorkSchedule().NextRun(),
            id = (Guid)Identification().ReadId(),
        };
        _journal.AddOperation(
            new CommandDefinition(
                sql,
                parameters,
                transaction: _journal.Transaction(),
                cancellationToken: _ct
            )
        );
        return status;
    }

    public Status<IParserLink> Put(IParserLink link)
    {
        Status<IParserLink> status = _parser.Put(link);
        if (status.IsFailure)
            return status;
        string sql = string.Intern(
            """
            INSERT INTO 
                parsers_management_module.parser_links (id, parser_id, name, url, activity, processed, total_seconds, hours, minutes, seconds)
            VALUES
                (@link_id, @parser_id, @link_name, @url, @activity, @link_processed, @link_total_seconds, @link_hours, @link_minutes, @link_seconds)
            """
        );
        var parameters = new
        {
            @link_id = (Guid)link.Identification().ReadId(),
            @parser_id = (Guid)link.Identification().OwnerIdentification().ReadId(),
            @link_name = link.Identification().ReadName().NameString().StringValue(),
            @url = link.ReadUrl().Read().StringValue(),
            @activity = link.Activity().Read(),
            @link_processed = link.WorkedStatistic().ProcessedAmount().Read().Read(),
            @link_total_seconds = link.WorkedStatistic().WorkedTime().Total().Read(),
            @link_hours = link.WorkedStatistic().WorkedTime().Hours().Read().Read(),
            @link_minutes = link.WorkedStatistic().WorkedTime().Minutes().Read().Read(),
            @link_seconds = link.WorkedStatistic().WorkedTime().Seconds().Read().Read(),
        };
        _journal.AddOperation(
            new CommandDefinition(
                sql,
                parameters,
                transaction: _journal.Transaction(),
                cancellationToken: _ct
            )
        );
        return status;
    }

    public Status<IParserLink> Drop(IParserLink link)
    {
        Status<IParserLink> status = _parser.Drop(link);
        if (status.IsFailure)
            return status;
        string sql = string.Intern(
            "DELETE FROM parsers_management_module.parser_links WHERE id = @link_id AND parser_id = @parser_link_id"
        );
        var parameters = new
        {
            link_id = (Guid)link.Identification().ReadId(),
            parser_link_id = (Guid)link.Identification().OwnerIdentification().ReadId(),
        };
        _journal.AddOperation(
            new CommandDefinition(
                sql,
                parameters,
                transaction: _journal.Transaction(),
                cancellationToken: _ct
            )
        );
        return status;
    }

    public Status<IParserLink> ChangeActivityOf(IParserLink link, bool nextActivity)
    {
        Status<IParserLink> status = _parser.ChangeActivityOf(link, nextActivity);
        if (status.IsFailure)
            return status;
        string sql = string.Intern(
            "UPDATE parsers_management_module.parser_links SET activity = @next_activity where id = @link_id and parser_id = @parser_link_id"
        );
        var parameters = new
        {
            next_activity = link.Activity().Read(),
            link_id = (Guid)link.Identification().ReadId(),
            parser_link_id = (Guid)link.Identification().OwnerIdentification().ReadId(),
        };
        _journal.AddOperation(
            new CommandDefinition(
                sql,
                parameters,
                transaction: _journal.Transaction(),
                cancellationToken: _ct
            )
        );
        return status;
    }

    public Status<IParserLink> Finish(IParserLink link, PositiveLong elapsed)
    {
        Status<IParserLink> status = _parser.Finish(link, elapsed);
        if (status.IsFailure)
            return status;
        string sql = string.Intern(
            """
            UPDATE parsers_management_module.parser_links 
            SET 
                total_seconds = @total, 
                hours = @hours, 
                minutes = @minutes, 
                seconds = @seconds 
            WHERE
                id = @link_id AND parser_id = @parser_link_id
            """
        );
        var parameters = new
        {
            total = link.WorkedStatistic().WorkedTime().Total().Read(),
            hours = link.WorkedStatistic().WorkedTime().Hours().Read().Read(),
            minutes = link.WorkedStatistic().WorkedTime().Minutes().Read().Read(),
            seconds = link.WorkedStatistic().WorkedTime().Seconds().Read().Read(),
            link_id = (Guid)link.Identification().ReadId(),
            parser_link_id = (Guid)link.Identification().OwnerIdentification().ReadId(),
        };
        _journal.AddOperation(
            new CommandDefinition(
                sql,
                parameters,
                transaction: _journal.Transaction(),
                cancellationToken: _ct
            )
        );
        return status;
    }

    public Status Stop()
    {
        Status status = _parser.Stop();
        if (status.IsFailure)
            return status;
        string sql = string.Intern(
            "UPDATE parsers_management_module.parsers SET state = @state WHERE id = @id"
        );
        var parameters = new
        {
            state = WorkState().Read().StringValue(),
            id = (Guid)Identification().ReadId(),
        };
        _journal.AddOperation(
            new CommandDefinition(
                sql,
                parameters,
                transaction: _journal.Transaction(),
                cancellationToken: _ct
            )
        );
        return status;
    }

    public Status Start()
    {
        Status status = _parser.Start();
        if (status.IsFailure)
            return status;
        string sql = string.Intern(
            """
            UPDATE parsers_management_module.parsers 
            SET 
                state = @state,
                processed = @processed,
                total_seconds = @total,
                hours = @hours,
                minutes = @minutes,
                seconds = @seconds
            WHERE id = @id
            """
        );
        var parameters = new
        {
            state = WorkState().Read().StringValue(),
            processed = WorkedStatistics().ProcessedAmount().Read().Read(),
            total = WorkedStatistics().WorkedTime().Total().Read(),
            hours = WorkedStatistics().WorkedTime().Hours().Read().Read(),
            minutes = WorkedStatistics().WorkedTime().Minutes().Read().Read(),
            seconds = WorkedStatistics().WorkedTime().Seconds().Read().Read(),
            id = (Guid)Identification().ReadId(),
        };
        _journal.AddOperation(
            new CommandDefinition(
                sql,
                parameters,
                transaction: _journal.Transaction(),
                cancellationToken: _ct
            )
        );
        return status;
    }

    public void Dispose() => _journal.Dispose();

    public ValueTask DisposeAsync() => _journal.DisposeAsync();

    public Task<Status> Save(CancellationToken ct = default) => _journal.Process(ct);
}
