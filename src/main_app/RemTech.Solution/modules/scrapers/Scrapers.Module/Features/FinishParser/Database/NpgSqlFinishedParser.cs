using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.FinishParser.Exceptions;
using Scrapers.Module.Features.FinishParser.Models;

namespace Scrapers.Module.Features.FinishParser.Database;

internal sealed class NpgSqlFinishedParser(NpgsqlDataSource dataSource) : IParserToFinishStorage
{
    private const string FetchSql = """
        SELECT name, wait_days, type FROM scrapers_module.scrapers
        WHERE name = @name AND type = @type;
        """;
    private const string NameParam = "@name";
    private const string TypeParam = "@type";
    private const string NameColumn = "name";
    private const string TypeColumn = "type";
    private const string WaitDaysColumn = "wait_days";

    public async Task<ParserToFinish> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = FetchSql;
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, parserName));
        command.Parameters.Add(new NpgsqlParameter<string>(TypeParam, parserType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows || !await reader.ReadAsync(ct))
            throw new CannotFindParserToFinishException(parserName, parserType);
        return new ParserToFinish(
            reader.GetString(reader.GetOrdinal(NameColumn)),
            reader.GetString(reader.GetOrdinal(TypeColumn)),
            reader.GetInt32(reader.GetOrdinal(WaitDaysColumn))
        );
    }

    private const string SaveSql = """
        UPDATE scrapers_module.scrapers
        SET total_seconds = @total,
            hours = @hours,
            minutes = @minutes,
            seconds = @seconds,
            last_run = @last_run,
            next_run = @next_run,
            state = @state
        WHERE name = @name AND type = @type;
        """;
    private const string StateParam = "@state";
    private const string TotalParam = "@total";
    private const string HourParam = "@hours";
    private const string MinutesParam = "@minutes";
    private const string SecondsParam = "@seconds";
    private const string LastRunParam = "@last_run";
    private const string NextRunParam = "@next_run";

    public async Task<FinishedParser> Save(FinishedParser parser, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SaveSql;
        command.Parameters.Add(new NpgsqlParameter<string>(StateParam, parser.ParserState));
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, parser.ParserName));
        command.Parameters.Add(new NpgsqlParameter<string>(TypeParam, parser.ParserType));
        command.Parameters.Add(new NpgsqlParameter<long>(TotalParam, parser.TotalElapsedSeconds));
        command.Parameters.Add(new NpgsqlParameter<int>(HourParam, parser.Hours));
        command.Parameters.Add(new NpgsqlParameter<int>(MinutesParam, parser.Minutes));
        command.Parameters.Add(new NpgsqlParameter<int>(SecondsParam, parser.Seconds));
        command.Parameters.Add(new NpgsqlParameter<DateTime>(LastRunParam, parser.LastRun));
        command.Parameters.Add(new NpgsqlParameter<DateTime>(NextRunParam, parser.NextRun));
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new CannotFindParserToFinishException(parser.ParserName, parser.ParserType)
            : parser;
    }
}
