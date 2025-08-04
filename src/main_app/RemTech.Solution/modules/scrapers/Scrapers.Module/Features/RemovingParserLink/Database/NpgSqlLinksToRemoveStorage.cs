using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.RemovingParserLink.Exceptions;
using Scrapers.Module.Features.RemovingParserLink.Models;

namespace Scrapers.Module.Features.RemovingParserLink.Database;

internal sealed class NpgSqlLinksToRemoveStorage(NpgsqlDataSource dataSource)
    : IRemovedParserLinksStorage
{
    public async Task<ParserLinkToRemove> Fetch(
        string linkName,
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT l.name as link_name, l.parser_name as parser_name, p.type as parser_type, p.state as parser_state
            FROM scrapers_module.scraper_links l
            INNER JOIN scrapers_module.scrapers p ON l.parser_name = p.name
            WHERE l.name = @linkName
            AND l.parser_name = @parserName
            AND p.type = @parserType
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@linkName", linkName));
        command.Parameters.Add(new NpgsqlParameter<string>("@parserName", parserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@parerType", parserType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new ParserLinkToRemoveNotFoundException(linkName, parserName, parserType);
        return new ParserLinkToRemove(
            reader.GetString(reader.GetOrdinal("link_name")),
            reader.GetString(reader.GetOrdinal("parser_name")),
            reader.GetString(reader.GetOrdinal("parser_type")),
            reader.GetString(reader.GetOrdinal("state"))
        );
    }

    public async Task<RemovedParserLink> Save(
        RemovedParserLink parserLink,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            DELETE FROM scrapers_module.scraper_links
            WHERE name = @name AND parser_name = @parserName;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parserLink.Name));
        command.Parameters.Add(new NpgsqlParameter<string>("@parserName", parserLink.ParserName));
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new ParserLinkToRemoveNotFoundException(
                parserLink.Name,
                parserLink.ParserName,
                parserLink.ParserType
            )
            : parserLink;
    }
}
