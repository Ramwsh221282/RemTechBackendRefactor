using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.ChangeParserState.Exception;
using Scrapers.Module.Features.ChangeParserState.Models;

namespace Scrapers.Module.Features.ChangeParserState.Database;

internal sealed class NpgSqlParserStateToChange(NpgsqlDataSource dataSource)
    : IParserStateToChangeStorage
{
    public async Task<ParserStateToChange> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT p.name, p.type, p.state FROM scrapers_module.scrapers p
            WHERE p.name = @name AND p.type = @type;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parserType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new ParserStateToChangeNotFoundException(parserName, parserType);
        if (!await reader.ReadAsync(ct))
            throw new ParserStateToChangeNotFoundException(parserName, parserType);
        return new ParserStateToChange(
            reader.GetString(reader.GetOrdinal("name")),
            reader.GetString(reader.GetOrdinal("type")),
            reader.GetString(reader.GetOrdinal("state"))
        );
    }

    public async Task<ParserWithChangedState> Save(
        ParserWithChangedState parser,
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
        command.Parameters.Add(new NpgsqlParameter<string>("@state", parser.NewState));
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new ParserStateToChangeNotFoundException(parser.ParserName, parser.ParserType)
            : parser;
    }
}
