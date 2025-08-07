using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.FinishParser.Exceptions;
using Scrapers.Module.Features.FinishParser.Models;

namespace Scrapers.Module.Features.FinishParser.Database;

internal sealed class NpgSqlFinishedParser(NpgsqlDataSource dataSource) : IParserToFinishStorage
{
    public async Task<ParserToFinish> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT name, wait_days, type FROM scrapers_module.scrapers
            WHERE name = @name AND type = @type;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parserType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows || !await reader.ReadAsync(ct))
            throw new CannotFindParserToFinishException(parserName, parserType);
        return new ParserToFinish(
            reader.GetString(reader.GetOrdinal("name")),
            reader.GetString(reader.GetOrdinal("type")),
            reader.GetInt32(reader.GetOrdinal("wait_days"))
        );
    }

    public async Task<FinishedParser> Save(FinishedParser parser, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scrapers
            SET total_seconds = @total,
                hours = @hours,
                minutes = @minutes,
                seconds = @seconds,
                last_run = @last_run,
                next_run = @next_run,
                state = @state
            WHERE name = @name AND type = @type;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@state", parser.ParserState));
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parser.ParserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parser.ParserType));
        command.Parameters.Add(new NpgsqlParameter<long>("@total", parser.TotalElapsedSeconds));
        command.Parameters.Add(new NpgsqlParameter<int>("@hours", parser.Hours));
        command.Parameters.Add(new NpgsqlParameter<int>("@minutes", parser.Minutes));
        command.Parameters.Add(new NpgsqlParameter<int>("@seconds", parser.Seconds));
        command.Parameters.Add(new NpgsqlParameter<DateTime>("@last_run", parser.LastRun));
        command.Parameters.Add(new NpgsqlParameter<DateTime>("@next_run", parser.NextRun));
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new CannotFindParserToFinishException(parser.ParserName, parser.ParserType)
            : parser;
    }
}
