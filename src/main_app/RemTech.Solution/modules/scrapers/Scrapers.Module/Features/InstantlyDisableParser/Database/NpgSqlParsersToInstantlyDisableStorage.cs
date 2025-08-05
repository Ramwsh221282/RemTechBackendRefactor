using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.InstantlyDisableParser.Exceptions;
using Scrapers.Module.Features.InstantlyDisableParser.Models;

namespace Scrapers.Module.Features.InstantlyDisableParser.Database;

internal sealed class NpgSqlParsersToInstantlyDisableStorage(NpgsqlDataSource dataSource)
    : IParsersToInstantlyDisableStorage
{
    public async Task<ParserToInstantlyDisable> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT name, type, state FROM scrapers_module.scrapers
            WHERE name = @name AND type = @type;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parserType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new UnableToFindParserToInstantlyDisableException(parserName, parserType);
        return new ParserToInstantlyDisable(
            reader.GetString(reader.GetOrdinal("name")),
            reader.GetString(reader.GetOrdinal("type")),
            reader.GetString(reader.GetOrdinal("state"))
        );
    }

    public async Task<InstantlyDisabledParser> Save(
        InstantlyDisabledParser parser,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scrapers
            SET state = @state
            WHERE name = @name AND type = @type;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parser.ParserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parser.ParserType));
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new UnableToFindParserToInstantlyDisableException(
                parser.ParserName,
                parser.ParserType
            )
            : parser;
    }
}
