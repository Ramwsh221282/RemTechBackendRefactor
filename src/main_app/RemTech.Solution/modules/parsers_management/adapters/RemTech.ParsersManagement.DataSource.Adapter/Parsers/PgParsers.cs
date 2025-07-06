using System.Data.Common;
using Dapper;
using Npgsql;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers.FromSqlConversion;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers.SelectConstants;
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
            FROM parsers p
            LEFT JOIN parser_links pl ON p.id = pl.parser_id
            WHERE p.name = @name
            """
        );
        var parameters = new { name = name.NameString().StringValue() };
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
        string sql = $"""
            SELECT
                {new SelectParserWithLeftJoinedLinks().Read()}
            FROM parsers p
            LEFT JOIN parser_links pl ON p.id = pl.parser_id
            WHERE p.id = @id
            """;
        var parameters = new { id = id.GuidValue() };
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
        string sql = $"""
            SELECT
                {new SelectParserWithLeftJoinedLinks().Read()}
            FROM parsers p
            LEFT JOIN parser_links pl ON p.id = pl.parser_id
            WHERE p.type = @type AND p.domain = @domain
            """;
        var parameters = new { type = type.Read().StringValue(), domain = domain.StringValue() };
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
        const string sql = """
            INSERT 
            INTO
                parsers (id, name, type, state, domain, processed, total_seconds, hours, minutes, seconds, wait_days, next_run, last_run)
            VALUES
                (@id, @name, @type, @state, @domain, @processed, @total_seconds, @hours, @minutes, @seconds, @wait_days, @next_run, @last_run)
            """;
        ParserIdentity identification = parser.Identification();
        var parameters = new
        {
            @id = identification.ReadId().GuidValue(),
            @name = identification.ReadName().NameString().StringValue(),
            @type = identification.ReadType().Read().StringValue(),
            @state = parser.WorkState().Read().StringValue(),
            @domain = parser.Domain().Read().NameString().StringValue(),
            @processed = parser.WorkedStatistics().ProcessedAmount().Read().Read(),
            @total_seconds = parser.WorkedStatistics().WorkedTime().Total().Read(),
            @hours = parser.WorkedStatistics().WorkedTime().Hours().Read().Read(),
            @minutes = parser.WorkedStatistics().WorkedTime().Minutes().Read().Read(),
            @seconds = parser.WorkedStatistics().WorkedTime().Seconds().Read().Read(),
            @wait_days = parser.WorkSchedule().WaitDays().Read(),
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
