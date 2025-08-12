using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.UpdateWaitDays.Exceptions;
using Scrapers.Module.Features.UpdateWaitDays.Models;

namespace Scrapers.Module.Features.UpdateWaitDays.Database;

internal sealed class NpgSqlParserWaitDaysToUpdateStorage(NpgsqlDataSource dataSource)
    : IParserWaitDaysToUpdateStorage
{
    private const string FetchSql = """
        SELECT name, type, state, next_run, wait_days
        FROM scrapers_module.scrapers
        WHERE name = @name AND type = @type 
        """;
    private const string NameParam = "@name";
    private const string TypeParam = "@type";
    private const string NameColumn = "name";
    private const string TypeColumn = "type";
    private const string StateColumn = "state";
    private const string NextRunColumn = "next_run";
    private const string WaitDaysColumn = "wait_days";

    public async Task<ParserWaitDaysToUpdate> Fetch(
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
            throw new ParserToUpdateWaitDaysNotFoundException(parserName, parserType);
        if (!await reader.ReadAsync(ct))
            throw new ParserToUpdateWaitDaysNotFoundException(parserName, parserType);
        return new ParserWaitDaysToUpdate(
            reader.GetString(reader.GetOrdinal(NameColumn)),
            reader.GetString(reader.GetOrdinal(TypeColumn)),
            reader.GetString(reader.GetOrdinal(StateColumn)),
            reader.GetInt32(reader.GetOrdinal(WaitDaysColumn)),
            reader.GetDateTime(reader.GetOrdinal(NextRunColumn))
        );
    }

    private const string SaveSql = """
        UPDATE scrapers_module.scrapers
        SET wait_days = @wait_days,
            next_run = @next_run
        WHERE name = @name AND type = @type;
        """;
    private const string WaitDaysParam = "@wait_days";
    private const string NextRunParam = "@next_run";

    public async Task<ParserWithUpdatedWaitDays> Save(
        ParserWithUpdatedWaitDays parser,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SaveSql;
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, parser.ParserName));
        command.Parameters.Add(new NpgsqlParameter<string>(TypeParam, parser.ParserType));
        command.Parameters.Add(new NpgsqlParameter<int>(WaitDaysParam, parser.WaitDays));
        command.Parameters.Add(new NpgsqlParameter<DateTime>(NextRunParam, parser.NextRun));
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new ParserToUpdateWaitDaysNotFoundException(
                parser.ParserName,
                parser.ParserType
            )
            : parser;
    }
}
