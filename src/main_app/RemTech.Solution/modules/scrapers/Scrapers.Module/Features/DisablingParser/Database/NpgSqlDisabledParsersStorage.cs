using Npgsql;
using Scrapers.Module.Features.DisablingParser.Exceptions;
using Scrapers.Module.Features.DisablingParser.Models;

namespace Scrapers.Module.Features.DisablingParser.Database;

internal sealed class NpgSqlDisabledParsersStorage(NpgsqlDataSource dataSource)
    : IDisabledParsersStorage
{
    public async Task<DisabledParser> SaveAsync(
        DisabledParser parser,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scrapers SET state = @state
            WHERE name = @name and type = @type;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@state", parser.State));
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parser.Name));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parser.Type));
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 0 ? throw new UnableToFindParserToDisableException() : parser;
    }
}
