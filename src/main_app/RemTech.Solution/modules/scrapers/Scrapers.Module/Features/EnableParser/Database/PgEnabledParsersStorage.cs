using Npgsql;
using Scrapers.Module.Features.EnableParser.Exceptions;
using Scrapers.Module.Features.EnableParser.Models;

namespace Scrapers.Module.Features.EnableParser.Database;

internal sealed class PgEnabledParsersStorage(NpgsqlDataSource dataSource) : IEnabledParsersStorage
{
    public async Task<EnabledParser> Save(
        EnabledParser parser,
        CancellationToken cancellationToken = default
    )
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scrapers SET state = @state
            WHERE name = @name and type = @type;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(
            cancellationToken
        );
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@state", parser.State));
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parser.Name));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parser.Type));
        int affected = await command.ExecuteNonQueryAsync(cancellationToken);
        return affected == 0 ? throw new ParserToEnableWasNotFoundException(parser) : parser;
    }
}
