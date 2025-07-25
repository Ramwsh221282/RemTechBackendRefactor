using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers.FromSqlConversion;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers.SelectConstants;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers;

public interface IPgCommandPlugin
{
    void Plug(NpgsqlCommand command);
}

public interface IPgCommand
{
    Task<NpgsqlCommand> Command();
}

public sealed class NonQueryCommand(IPgCommand command)
{
    public async Task Executed(CancellationToken ct)
    {
        await using NpgsqlCommand created = await command.Command();
        int execution = await created.ExecuteNonQueryAsync(ct);
    }
}

public sealed class DbDataReaderCommand(IPgCommand command)
{
    public async Task<DbDataReader> Reader(CancellationToken ct = default)
    {
        await using NpgsqlCommand createdCommand = await command.Command();
        return await createdCommand.ExecuteReaderAsync(ct);
    }
}

public sealed class PreparedCommand(IPgCommand origin) : IPgCommand
{
    private NpgsqlCommand? _command;
    private bool _prepared;

    public async Task<NpgsqlCommand> Command()
    {
        if (_prepared && _command != null)
            return _command;
        _command = await origin.Command();
        _prepared = true;
        return _command;
    }
}

public sealed class TransactionedCommandPlugin(
    NpgsqlTransaction transaction,
    IPgCommandPlugin origin
) : IPgCommandPlugin
{
    private bool _transactionSet;

    public void Plug(NpgsqlCommand command)
    {
        if (_transactionSet)
            return;
        command.Transaction = transaction;
        _transactionSet = true;
        origin.Plug(command);
    }
}

public sealed class ParametrizedCommandPlugin(IPgCommandPlugin origin) : IPgCommandPlugin
{
    private readonly List<NpgsqlParameter> _parameters = [];
    private bool _prepared;

    public void Plug(NpgsqlCommand command)
    {
        if (_prepared)
            return;
        foreach (NpgsqlParameter parameter in _parameters)
            command.Parameters.Add(parameter);
        _prepared = true;
        origin.Plug(command);
    }

    public ParametrizedCommandPlugin With<T>(string name, T value)
    {
        NpgsqlParameter<T> parameter = new(name, value);
        _parameters.Add(parameter);
        return this;
    }
}

public sealed class SqlCommandPlugin(string sql) : IPgCommandPlugin
{
    private bool _prepared;

    public void Plug(NpgsqlCommand command)
    {
        if (_prepared)
            return;
        command.CommandText = sql;
        _prepared = true;
    }
}

public sealed class CustomizedCommand(IPgCommandPlugin plugin, IPgCommand command) : IPgCommand
{
    public async Task<NpgsqlCommand> Command()
    {
        NpgsqlCommand standard = await command.Command();
        plugin.Plug(standard);
        return standard;
    }
}

public sealed class ConnectedCommand(NpgsqlConnection connection) : IPgCommand
{
    public Task<NpgsqlCommand> Command() =>
        Task.FromResult(new NpgsqlCommand(string.Empty, connection));
}

public sealed class PgParserCommands(NpgsqlConnection connection)
{
    public IPgCommand FindByNameCommand(Name name)
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
        return new PreparedCommand(
            new CustomizedCommand(
                new ParametrizedCommandPlugin(new SqlCommandPlugin(sql)).With<string>(
                    "@name",
                    name.NameString()
                ),
                new ConnectedCommand(connection)
            )
        );
    }

    public IPgCommand FindByIdCommand(NotEmptyGuid id)
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
        return new PreparedCommand(
            new CustomizedCommand(
                new ParametrizedCommandPlugin(new SqlCommandPlugin(sql)).With<Guid>("@id", id),
                new ConnectedCommand(connection)
            )
        );
    }

    public IPgCommand FindByDomainAndTypeCommand(ParsingType type, NotEmptyString domain)
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
        return new PreparedCommand(
            new CustomizedCommand(
                new ParametrizedCommandPlugin(new SqlCommandPlugin(sql))
                    .With<string>("@type", type.Read())
                    .With<string>("@domain", domain),
                new ConnectedCommand(connection)
            )
        );
    }

    public IPgCommand AddParserCommand(IParser parser)
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
        return new CustomizedCommand(
            new ParametrizedCommandPlugin(new SqlCommandPlugin(sql))
                .With<Guid>("@id", parser.Identification().ReadId())
                .With<string>("@name", parser.Identification().ReadName())
                .With<string>("@type", parser.Identification().ReadType().Read())
                .With<string>("@state", parser.WorkState())
                .With<string>("@domain", parser.Domain().Read())
                .With<int>("@processed", parser.WorkedStatistics().ProcessedAmount())
                .With<long>("@total_seconds", parser.WorkedStatistics().WorkedTime().Total())
                .With<int>("@hours", parser.WorkedStatistics().WorkedTime().Hours())
                .With<int>("@minutes", parser.WorkedStatistics().WorkedTime().Minutes())
                .With<int>("@seconds", parser.WorkedStatistics().WorkedTime().Seconds())
                .With<int>("@wait_days", parser.WorkSchedule().WaitDays())
                .With("@next_run", parser.WorkSchedule().NextRun().Date)
                .With("@last_run", parser.WorkSchedule().LastRun().Date),
            new ConnectedCommand(connection)
        );
    }
}

public sealed class PgParsers : IParsers
{
    private readonly NpgsqlDataSource _source;

    public PgParsers(NpgsqlDataSource source)
    {
        _source = source;
    }

    public async Task<Status<IParser>> Find(Name name, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await _source.OpenConnectionAsync(ct);
        await using DbDataReader reader = await new DbDataReaderCommand(
            new PgParserCommands(connection).FindByNameCommand(name)
        ).Reader(ct);
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
        await using NpgsqlConnection connection = await _source.OpenConnectionAsync(ct);
        await using DbDataReader reader = await new DbDataReaderCommand(
            new PgParserCommands(connection).FindByIdCommand(id)
        ).Reader(ct);
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
        await using NpgsqlConnection connection = await _source.OpenConnectionAsync(ct);
        await using DbDataReader reader = await new DbDataReaderCommand(
            new PgParserCommands(connection).FindByDomainAndTypeCommand(type, domain)
        ).Reader(ct);
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
        await using NpgsqlConnection connection = await _source.OpenConnectionAsync(ct);
        await new NonQueryCommand(
            new PgParserCommands(connection).AddParserCommand(parser)
        ).Executed(ct);
        return parser.Success();
    }

    public void Dispose() => _source.Dispose();

    public async ValueTask DisposeAsync() => await _source.DisposeAsync();
}
