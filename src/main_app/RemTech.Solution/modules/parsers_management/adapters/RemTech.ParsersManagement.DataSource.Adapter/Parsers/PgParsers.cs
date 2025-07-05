using System.Data.Common;
using Dapper;
using Npgsql;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers;

public sealed class PgParsers : IParsers
{
    private readonly PostgreSqlEngine _engine;

    public PgParsers(PostgreSqlEngine engine) => _engine = engine;

    public Task<Status<IParser>> Find(Name name, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Status<IParser>> Find(NotEmptyGuid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Status<IParser>> Find(
        ParsingType type,
        NotEmptyString domain,
        CancellationToken ct = default
    )
    {
        const string sql = """
            SELECT
                p.id,
                p.name,
                p.type,
                p.state,
                p.domain,
                p.processed,
                p.total_seconds,
                p.hours,
                p.minutes,
                p.seconds,
                p.wait_days,
                p.next_run,
                p.last_run,
                pl.id as link_id,
                pl.parser_id,
                pl.name as link_name,
                pl.url,
                pl.activity,
                pl.processed as link_processed,
                pl.total_seconds as link_total_seconds,
                pl.hours as link_hours,
                pl.minutes as link_minutes,
                pl.seconds as link_seconds
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
            : parsers[0].Success();
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
}
