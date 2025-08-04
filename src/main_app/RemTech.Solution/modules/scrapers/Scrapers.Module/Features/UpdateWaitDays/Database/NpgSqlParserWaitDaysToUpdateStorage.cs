using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.UpdateWaitDays.Endpoint;
using Scrapers.Module.Features.UpdateWaitDays.Exceptions;

namespace Scrapers.Module.Features.UpdateWaitDays.Database;

internal sealed class NpgSqlParserWaitDaysToUpdateStorage(NpgsqlDataSource dataSource)
    : IParserWaitDaysToUpdateStorage
{
    public async Task<ParserWaitDaysToUpdate> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT name, type, state, next_run
            FROM scrapers_module.scrapers
            WHERE name = @name AND type = @type AND 
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parserType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new ParserToUpdateWaitDaysNotFoundException(parserName, parserType);
        return new ParserWaitDaysToUpdate(
            reader.GetString(reader.GetOrdinal("name")),
            reader.GetString(reader.GetOrdinal("type")),
            reader.GetString(reader.GetOrdinal("state")),
            reader.GetDateTime(reader.GetOrdinal("next_run"))
        );
    }

    public async Task<ParserWithUpdatedWaitDays> Save(
        ParserWithUpdatedWaitDays parser,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scrapers
            SET wait_days = @wait_days,
                next_run = @next_run
            WHERE name = @name AND type = @type;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parser.ParserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parser.ParserType));
        command.Parameters.Add(new NpgsqlParameter<int>("@wait_days", parser.WaitDays));
        command.Parameters.Add(new NpgsqlParameter<DateTime>("@next_run", parser.NextRun));
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new ParserToUpdateWaitDaysNotFoundException(
                parser.ParserName,
                parser.ParserType
            )
            : parser;
    }
}
