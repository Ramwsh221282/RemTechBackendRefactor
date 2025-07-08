using System.Data.Common;
using Dapper;
using Npgsql;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers.FromSqlConversion;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers.SelectConstants;
using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers;

public sealed class PgParsers : IParsers
{
    private readonly PostgreSqlEngine _engine;

    public PgParsers(PostgreSqlEngine engine) => _engine = engine;

    public async Task<Status<IParser>> Find(Name name, CancellationToken ct = default)
    {
        string sql = string.Intern(
            $"""
            SELECT
                {new SelectParserWithLeftJoinedLinks().Read()}
            FROM parsers_management_module.parsers p
            LEFT JOIN parsers_management_module.parser_links pl ON p.id = pl.parser_id
            WHERE p.name = @name
            """
        );
        var parameters = new { name = (string)name.NameString() };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        await using NpgsqlConnection connection = await _engine.Connect(ct);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        List<IParser> parsers = [];
        while (await reader.ReadAsync(ct))
        {
            IParser parser = await new ParserFromSqlRow(reader).Read();
            parsers.Add(parser);
        }
        return parsers.Count == 0
            ? new ParserWithNameNotFoundError(name)
            : new ValidParser(parsers[0]);
    }

    public async Task<Status<IParser>> Find(NotEmptyGuid id, CancellationToken ct = default)
    {
        string sql = string.Intern(
            $"""
            SELECT
                {new SelectParserWithLeftJoinedLinks().Read()}
            FROM parsers_management_module.parsers p
            LEFT JOIN parsers_management_module.parser_links pl ON p.id = pl.parser_id
            WHERE p.id = @id
            """
        );
        var parameters = new { id = (Guid)id };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        await using NpgsqlConnection connection = await _engine.Connect(ct);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        List<IParser> parsers = [];
        while (await reader.ReadAsync(ct))
        {
            IParser parser = await new ParserFromSqlRow(reader).Read();
            parsers.Add(parser);
        }

        return parsers.Count == 0 ? new ParserWithIdNotFoundError(id) : new ValidParser(parsers[0]);
    }

    public async Task<Status<IParser>> Find(
        ParsingType type,
        NotEmptyString domain,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            $"""
            SELECT
                {new SelectParserWithLeftJoinedLinks().Read()}
            FROM parsers_management_module.parsers p
            LEFT JOIN parsers_management_module.parser_links pl ON p.id = pl.parser_id
            WHERE p.type = @type AND p.domain = @domain
            """
        );
        var parameters = new { type = (string)type.Read(), domain = (string)domain };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        await using NpgsqlConnection connection = await _engine.Connect(ct);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        List<IParser> parsers = [];
        while (await reader.ReadAsync(ct))
        {
            IParser parser = await new ParserFromSqlRow(reader).Read();
            parsers.Add(parser);
        }

        return parsers.Count == 0
            ? new ParserWithTypeAndDomainNotFoundError(type, domain)
            : new ValidParser(parsers[0]);
    }

    public async Task<Status> Add(IParser parser, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            INSERT 
            INTO
                parsers_management_module.parsers (id, name, type, state, domain, processed, total_seconds, hours, minutes, seconds, wait_days, next_run, last_run)
            VALUES
                (@id, @name, @type, @state, @domain, @processed, @total_seconds, @hours, @minutes, @seconds, @wait_days, @next_run, @last_run)
            """
        );
        ParserIdentity identification = parser.Identification();
        var parameters = new
        {
            @id = (Guid)identification.ReadId(),
            @name = (string)identification.ReadName().NameString(),
            @type = (string)identification.ReadType().Read(),
            @state = (string)parser.WorkState().Read(),
            @domain = (string)parser.Domain().Read().NameString(),
            @processed = (int)parser.WorkedStatistics().ProcessedAmount(),
            @total_seconds = (long)parser.WorkedStatistics().WorkedTime().Total(),
            @hours = (int)parser.WorkedStatistics().WorkedTime().Hours(),
            @minutes = (int)parser.WorkedStatistics().WorkedTime().Minutes(),
            @seconds = (int)parser.WorkedStatistics().WorkedTime().Seconds(),
            @wait_days = (int)parser.WorkSchedule().WaitDays(),
            @next_run = parser.WorkSchedule().NextRun(),
            @last_run = parser.WorkSchedule().LastRun(),
        };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        await using NpgsqlConnection connection = await _engine.Connect(ct);
        await connection.ExecuteAsync(command);
        return parser.Success();
    }

    public void Dispose() => _engine.Dispose();

    public async ValueTask DisposeAsync() => await _engine.DisposeAsync();
}
