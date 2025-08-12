using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.UpdateParserLink.Exceptions;
using Scrapers.Module.Features.UpdateParserLink.Models;

namespace Scrapers.Module.Features.UpdateParserLink.Database;

internal sealed class NpgSqlParserLinkToUpdateStorage(NpgsqlDataSource dataSource)
    : IParserLinkToUpdateStorage
{
    private const string FetchSql = """
        SELECT 
            p.domain as parser_domain, 
            l.parser_name as link_parser_name, 
            l.parser_type as link_parser_type, 
            l.name as link_name, 
            l.url as link_url 
        FROM scrapers_module.scraper_links l
        INNER JOIN scrapers_module.scrapers p ON l.parser_name = p.name AND l.parser_type = p.type
        WHERE l.parser_name = @parser_name AND l.parser_type = @parser_type AND l.name = @name AND l.url = @url
        """;
    private const string ParserNameParam = "@parser_name";
    private const string ParserTypeParam = "@parser_type";
    private const string NameParam = "@name";
    private const string UrlParam = "@url";
    private const string LinkParserNameColumn = "link_parser_name";
    private const string LinkParserTypeColumn = "link_parser_type";
    private const string ParserDomainColumn = "parser_domain";
    private const string LinkUrlColumn = "link_url";
    private const string LinkNameColumn = "link_name";

    public async Task<ParserLinkToUpdate> Fetch(
        string parserName,
        string parserType,
        string linkName,
        string linkUrl,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = FetchSql;
        command.Parameters.Add(new NpgsqlParameter<string>(ParserNameParam, parserName));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserTypeParam, parserType));
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, linkName));
        command.Parameters.Add(new NpgsqlParameter<string>(UrlParam, linkUrl));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new ParserLinkToUpdateNotFoundException(parserName, parserType, linkName);
        if (!await reader.ReadAsync(ct))
            throw new ParserLinkToUpdateNotFoundException(parserName, parserType, linkName);
        string dbParserName = reader.GetString(reader.GetOrdinal(LinkParserNameColumn));
        string dbParserType = reader.GetString(reader.GetOrdinal(LinkParserTypeColumn));
        string dbParserDomain = reader.GetString(reader.GetOrdinal(ParserDomainColumn));
        string dbLinkUrl = reader.GetString(reader.GetOrdinal(LinkUrlColumn));
        string dbLinkName = reader.GetString(reader.GetOrdinal(LinkNameColumn));
        return new ParserLinkToUpdate(
            dbParserName,
            dbParserType,
            dbParserDomain,
            dbLinkUrl,
            dbLinkName
        );
    }

    private const string OldNameParam = "@old_name";
    private const string OldUrlParam = "@old_url";

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
        command.Parameters.Add(new NpgsqlParameter<string>(ParserNameParam, parserName));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserTypeParam, parserType));
        command.Parameters.Add(new NpgsqlParameter<string>(OldNameParam, oldLinkName));
        command.Parameters.Add(new NpgsqlParameter<string>(OldUrlParam, oldLinkUrl));
        await command.ExecuteNonQueryAsync(ct);
    }

    private const string WithOnlyNameUpdateSql = """
        UPDATE scrapers_module.scraper_links
        SET name = @name
        WHERE parser_name = @parser_name
              AND
              parser_type = @parser_type
              AND
              name = @old_name
              AND
              url = @old_url
        """;

    private static void WithOnlyNameUpdate(string name, NpgsqlCommand command)
    {
        command.CommandText = WithOnlyNameUpdateSql;
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, name));
    }

    private const string WithOnlyUrlUpdateSql = """
        UPDATE scrapers_module.scraper_links
        SET url = @url
        WHERE parser_name = @parser_name
              AND
              parser_type = @parser_type
              AND
              name = @old_name
              AND
              url = @old_url
        """;

    private static void WithOnlyUrlUpdate(string url, NpgsqlCommand command)
    {
        command.CommandText = WithOnlyUrlUpdateSql;
        command.Parameters.Add(new NpgsqlParameter<string>(UrlParam, url));
    }

    private const string WithNameAndUrlUpdateSql = """
        UPDATE scrapers_module.scraper_links
        SET url = @url, name = @name
        WHERE parser_name = @parser_name
              AND
              parser_type = @parser_type
              AND
              name = @old_name
              AND
              url = @old_url
        """;

    private static void WithUrlAndNameUpdate(string url, string name, NpgsqlCommand command)
    {
        command.CommandText = WithNameAndUrlUpdateSql;
        command.Parameters.Add(new NpgsqlParameter<string>(UrlParam, url));
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, name));
    }
}
