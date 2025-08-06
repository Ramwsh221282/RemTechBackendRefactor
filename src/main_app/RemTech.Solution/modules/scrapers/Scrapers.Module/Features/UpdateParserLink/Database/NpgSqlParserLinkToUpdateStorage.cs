using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.UpdateParserLink.Exceptions;
using Scrapers.Module.Features.UpdateParserLink.Models;

namespace Scrapers.Module.Features.UpdateParserLink.Database;

internal sealed class NpgSqlParserLinkToUpdateStorage(NpgsqlDataSource dataSource)
    : IParserLinkToUpdateStorage
{
    public async Task<ParserLinkToUpdate> Fetch(
        string parserName,
        string parserType,
        string linkName,
        string linkUrl,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT 
                p.domain as parser_domain, 
                l.parser_name as link_parser_name, 
                l.parser_type as link_parser_type, 
                l.name as link_name, 
                l.url as link_url 
            FROM scrapers_module.scraper_links l
            INNER JOIN scrapers_module.scrapers p ON l.parser_name = p.name AND l.parser_type = p.type
            WHERE l.parser_name = @parser_name AND l.parser_type = @parser_type AND l.name = @name AND l.url = @url
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("parser_name", parserName));
        command.Parameters.Add(new NpgsqlParameter<string>("parser_type", parserType));
        command.Parameters.Add(new NpgsqlParameter<string>("name", linkName));
        command.Parameters.Add(new NpgsqlParameter<string>("url", linkUrl));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new ParserLinkToUpdateNotFoundException(parserName, parserType, linkName);
        if (!await reader.ReadAsync(ct))
            throw new ParserLinkToUpdateNotFoundException(parserName, parserType, linkName);
        string dbParserName = reader.GetString(reader.GetOrdinal("link_parser_name"));
        string dbParserType = reader.GetString(reader.GetOrdinal("link_parser_type"));
        string dbParserDomain = reader.GetString(reader.GetOrdinal("parser_domain"));
        string dbLinkUrl = reader.GetString(reader.GetOrdinal("link_url"));
        string dbLinkName = reader.GetString(reader.GetOrdinal("link_name"));
        return new ParserLinkToUpdate(
            dbParserName,
            dbParserType,
            dbParserDomain,
            dbLinkUrl,
            dbLinkName
        );
    }

    public async Task Save(
        string parserName,
        string parserType,
        string linkName,
        string linkUrl,
        string oldLinkName,
        string oldLinkUrl,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        if (linkName == oldLinkName && linkUrl == oldLinkUrl)
            return;
        if (linkName != oldLinkName && linkUrl != oldLinkUrl)
            WithUrlAndNameUpdate(linkUrl, linkName, command);
        if (linkName != oldLinkName && linkUrl == oldLinkUrl)
            WithOnlyNameUpdate(linkName, command);
        if (linkName == oldLinkName && linkUrl != oldLinkUrl)
            WithOnlyUrlUpdate(linkUrl, command);
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_name", parserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_type", parserType));
        command.Parameters.Add(new NpgsqlParameter<string>("@old_name", oldLinkName));
        command.Parameters.Add(new NpgsqlParameter<string>("@old_url", oldLinkUrl));
        await command.ExecuteNonQueryAsync(ct);
    }

    private static void WithOnlyNameUpdate(string name, NpgsqlCommand command)
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scraper_links
            SET name = @name
            WHERE parser_name = @parser_name
                  AND
                  parser_type = @parser_type
                  AND
                  name = @old_name
                  AND
                  url = @old_url
            """
        );
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", name));
    }

    private static void WithOnlyUrlUpdate(string url, NpgsqlCommand command)
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scraper_links
            SET url = @url
            WHERE parser_name = @parser_name
                  AND
                  parser_type = @parser_type
                  AND
                  name = @old_name
                  AND
                  url = @old_url
            """
        );
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@url", url));
    }

    private static void WithUrlAndNameUpdate(string url, string name, NpgsqlCommand command)
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scraper_links
            SET url = @url, name = @name
            WHERE parser_name = @parser_name
                  AND
                  parser_type = @parser_type
                  AND
                  name = @old_name
                  AND
                  url = @old_url
            """
        );
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@url", url));
        command.Parameters.Add(new NpgsqlParameter<string>("@name", name));
    }
}
