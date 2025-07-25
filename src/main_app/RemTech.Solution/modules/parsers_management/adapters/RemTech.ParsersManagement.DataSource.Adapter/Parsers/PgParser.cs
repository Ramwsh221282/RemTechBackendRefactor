using Npgsql;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers;

public sealed class PgParser : IParser
{
    private readonly IParser _parser;
    private readonly NpgsqlConnection _connection;

    public PgParser(IParser parser, NpgsqlConnection connection)
    {
        _parser = parser;
        _connection = connection;
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
        string parsersSql = string.Intern(
            "UPDATE parsers_management_module.parsers SET processed = @parser_processed WHERE id = @parser_id"
        );
        string linksSql = string.Intern(
            "UPDATE parsers_management_module.parser_links SET processed = @link_processed WHERE id = @link_id AND parser_id = @parser_link_id"
        );
        new ExecutedPgCommands()
            .With(
                new ExecutedPgCommand(
                    new PreparedPgCommand(
                        new ParametrizingPgCommand(
                            new PgCommand(_connection, string.Intern(parsersSql))
                        )
                            .With(
                                string.Intern("@parser_processed"),
                                status.Value.CurrentProcessed()
                            )
                            .With(string.Intern("@parser_id"), status.Value.IdOfIncreased())
                    )
                )
            )
            .With(
                new ExecutedPgCommand(
                    new PreparedPgCommand(
                        new ParametrizingPgCommand(
                            new PgCommand(_connection, string.Intern(linksSql))
                        )
                            .With(
                                string.Intern("@link_processed"),
                                status.Value.LinkIncreasement().CurrentProcessed()
                            )
                            .With(
                                string.Intern("@link_id"),
                                status.Value.LinkIncreasement().IdOfIncreased()
                            )
                            .With(string.Intern("@parser_link_id"), status.Value.IdOfIncreased())
                    )
                )
            )
            .Executed();
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
        new ExecutedPgCommand(
            new PreparedPgCommand(
                new ParametrizingPgCommand(new PgCommand(_connection, sql))
                    .With<string>(string.Intern("@state"), WorkState())
                    .With<Guid>(string.Intern("@id"), Identification().ReadId())
            )
        ).Executed();
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
        new ExecutedPgCommand(
            new PreparedPgCommand(
                new ParametrizingPgCommand(new PgCommand(_connection, sql))
                    .With<string>(string.Intern("@state"), WorkState())
                    .With<Guid>(string.Intern("@id"), Identification().ReadId())
            )
        ).Executed();
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
        new ExecutedPgCommand(
            new PreparedPgCommand(
                new ParametrizingPgCommand(new PgCommand(_connection, sql))
                    .With<string>(string.Intern("@state"), WorkState())
                    .With<Guid>(string.Intern("@id"), Identification().ReadId())
            )
        ).Executed();
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
        new ExecutedPgCommand(
            new PreparedPgCommand(
                new ParametrizingPgCommand(new PgCommand(_connection, sql))
                    .With(string.Intern("@wait_days"), WorkSchedule().WaitDays().Read())
                    .With(string.Intern("@next_run"), WorkSchedule().NextRun())
                    .With<Guid>(string.Intern("@id"), Identification().ReadId())
            )
        ).Executed();
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
        ParserLinkIdentity linkIdentity = link.Identification();
        ParserLinkStatistic linkStatistics = link.WorkedStatistic();
        new ExecutedPgCommand(
            new PreparedPgCommand(
                new ParametrizingPgCommand(new PgCommand(_connection, sql))
                    .With<Guid>(string.Intern("@link_id"), linkIdentity.ReadId())
                    .With<Guid>(
                        string.Intern("@parser_id"),
                        linkIdentity.OwnerIdentification().ReadId()
                    )
                    .With<string>(string.Intern("@link_name"), linkIdentity.ReadName())
                    .With<string>(string.Intern("@url"), link.ReadUrl())
                    .With<bool>(string.Intern("@activity"), link.Activity())
                    .With<int>(string.Intern("@link_processed"), linkStatistics.ProcessedAmount())
                    .With<long>(
                        string.Intern("@link_total_seconds"),
                        linkStatistics.WorkedTime().Total()
                    )
                    .With<int>(
                        string.Intern("@link_hours"),
                        link.WorkedStatistic().WorkedTime().Hours()
                    )
                    .With<int>(
                        string.Intern("@link_minutes"),
                        linkStatistics.WorkedTime().Minutes()
                    )
                    .With<int>(
                        string.Intern("@link_seconds"),
                        linkStatistics.WorkedTime().Seconds()
                    )
            )
        ).Executed();
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
        Guid linkId = link.Identification().ReadId();
        Guid parserId = link.Identification().OwnerIdentification().ReadId();
        new ExecutedPgCommand(
            new PreparedPgCommand(
                new ParametrizingPgCommand(new PgCommand(_connection, sql))
                    .With(string.Intern("@link_id"), linkId)
                    .With(string.Intern("@parser_link_id"), parserId)
            )
        ).Executed();
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
        bool activity = link.Activity().Read();
        Guid linkId = link.Identification().ReadId();
        Guid parserId = link.Identification().OwnerIdentification().ReadId();
        new ExecutedPgCommand(
            new PreparedPgCommand(
                new ParametrizingPgCommand(new PgCommand(_connection, sql))
                    .With(string.Intern("@next_activity"), activity)
                    .With(string.Intern("@link_id"), linkId)
                    .With(string.Intern("@parser_link_id"), parserId)
            )
        ).Executed();
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
        long total = link.WorkedStatistic().WorkedTime().Total().Read();
        int hours = link.WorkedStatistic().WorkedTime().Hours().Read().Read();
        int minutes = link.WorkedStatistic().WorkedTime().Minutes().Read();
        int seconds = link.WorkedStatistic().WorkedTime().Seconds().Read();
        Guid linkId = link.Identification().ReadId();
        Guid parserId = link.Identification().OwnerIdentification().ReadId();
        new ExecutedPgCommand(
            new PreparedPgCommand(
                new ParametrizingPgCommand(new PgCommand(_connection, sql))
                    .With(string.Intern("@link_id"), linkId)
                    .With(string.Intern("@parser_link_id"), parserId)
                    .With(string.Intern("@total"), total)
                    .With(string.Intern("@hours"), hours)
                    .With(string.Intern("@minutes"), minutes)
                    .With(string.Intern("@seconds"), seconds)
            )
        ).Executed();
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
        string state = WorkState();
        Guid id = Identification().ReadId();
        new ExecutedPgCommand(
            new PreparedPgCommand(
                new ParametrizingPgCommand(new PgCommand(_connection, sql))
                    .With(string.Intern("@id"), id)
                    .With(string.Intern("@state"), state)
            )
        ).Executed();
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
        string state = WorkState();
        int processed = WorkedStatistics().ProcessedAmount();
        long total = WorkedStatistics().ProcessedAmount();
        int hours = WorkedStatistics().WorkedTime().Hours();
        int minutes = WorkedStatistics().WorkedTime().Minutes();
        int seconds = WorkedStatistics().WorkedTime().Seconds();
        Guid id = Identification().ReadId();
        new ExecutedPgCommand(
            new PreparedPgCommand(
                new ParametrizingPgCommand(new PgCommand(_connection, sql))
                    .With(string.Intern("@id"), id)
                    .With(string.Intern("@state"), state)
                    .With(string.Intern("@processed"), processed)
                    .With(string.Intern("@total"), total)
                    .With(string.Intern("@hours"), hours)
                    .With(string.Intern("@minutes"), minutes)
                    .With(string.Intern("@seconds"), seconds)
            )
        ).Executed();
        return status;
    }

    public void Dispose() => _connection.Dispose();

    public ValueTask DisposeAsync() => _connection.DisposeAsync();
}
