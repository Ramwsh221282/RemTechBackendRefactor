using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.ChangeLinkActivity.Exceptions;
using Scrapers.Module.Features.ChangeLinkActivity.Models;

namespace Scrapers.Module.Features.ChangeLinkActivity.Database;

internal sealed class NpgSqlLinkActivityToChangeStorage(NpgsqlDataSource dataSource)
    : ILinkActivityToChangeStorage
{
    public async Task<LinkActivityToChange> Fetch(
        string name,
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT 
            l.name as link_name, 
            l.parser_name as link_parser_name, 
            l.activity as link_activity,
            p.type as parser_type,
            p.state as parser_state
            FROM scrapers_module.scraper_links l
            INNER JOIN scrapers_module.scrapers p
            ON l.parser_name = p.name
            WHERE l.name = @name AND l.parser_name = @parser_name AND p.type = @type;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using var command = new NpgsqlCommand(sql, connection);
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", name));
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_name", parserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parserType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new LinkActivityToChangeNotFoundException(name, parserName, parserType);
        if (!await reader.ReadAsync(ct))
            throw new LinkActivityToChangeNotFoundException(name, parserName, parserType);
        string linkName = reader.GetString(reader.GetOrdinal("link_name"));
        string parserLinkName = reader.GetString(reader.GetOrdinal("link_parser_name"));
        bool activity = reader.GetBoolean(reader.GetOrdinal("link_activity"));
        string linkParserType = reader.GetString(reader.GetOrdinal("parser_type"));
        string linkParserState = reader.GetString(reader.GetOrdinal("parser_state"));
        LinkActivityToChange link = new(
            linkName,
            parserLinkName,
            linkParserType,
            activity,
            linkParserState
        );
        return link;
    }

    public async Task<LinkWithChangedActivity> Save(
        LinkWithChangedActivity link,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scraper_links
            SET activity = @activity
            WHERE name = @name AND parser_name = @parser_name;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using var command = new NpgsqlCommand(sql, connection);
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", link.Name));
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_name", link.ParserName));
        command.Parameters.Add(new NpgsqlParameter<bool>("@activity", link.CurrentActivity));
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new LinkActivityToChangeNotFoundException(
                link.Name,
                link.ParserName,
                link.ParserType
            )
            : link;
    }
}
