using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.ChangeParserState.Exception;
using Scrapers.Module.Features.ChangeParserState.Models;

namespace Scrapers.Module.Features.ChangeParserState.Database;

internal sealed class NpgSqlParserStateToChange(NpgsqlDataSource dataSource)
    : IParserStateToChangeStorage
{
    private const string FetchSql = """
        SELECT p.name, p.type, p.state FROM scrapers_module.scrapers p
        WHERE p.name = @name AND p.type = @type;
        """;

    private const string NameParam = "@name";
    private const string TypeParam = "@type";
    private const string NameColumn = "name";
    private const string TypeColumn = "type";
    private const string StateColumn = "state";

    public async Task<ParserStateToChange> Fetch(
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
        if (!reader.HasRows)
            throw new ParserStateToChangeNotFoundException(parserName, parserType);
        if (!await reader.ReadAsync(ct))
            throw new ParserStateToChangeNotFoundException(parserName, parserType);
        return new ParserStateToChange(
            reader.GetString(reader.GetOrdinal(NameColumn)),
            reader.GetString(reader.GetOrdinal(TypeColumn)),
            reader.GetString(reader.GetOrdinal(StateColumn))
        );
    }

    private const string SaveSql = """
        UPDATE scrapers_module.scrapers
        SET state = @state
        WHERE name = @name AND type = @type;
        """;

    private const string StateParam = "@state";

    public async Task<ParserWithChangedState> Save(
        ParserWithChangedState parser,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SaveSql;
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, parser.ParserName));
        command.Parameters.Add(new NpgsqlParameter<string>(TypeParam, parser.ParserType));
        command.Parameters.Add(new NpgsqlParameter<string>(StateParam, parser.NewState));
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new ParserStateToChangeNotFoundException(parser.ParserName, parser.ParserType)
            : parser;
    }
}
