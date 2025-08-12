using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.RemovingParserLink.Exceptions;
using Scrapers.Module.Features.RemovingParserLink.Models;

namespace Scrapers.Module.Features.RemovingParserLink.Database;

internal sealed class NpgSqlLinksToRemoveStorage(NpgsqlDataSource dataSource)
    : IRemovedParserLinksStorage
{
    private const string FetchSql = """
        SELECT l.name as link_name, l.parser_name as parser_name, l.url as link_url, p.type as parser_type, p.state as parser_state
        FROM scrapers_module.scraper_links l
        INNER JOIN scrapers_module.scrapers p ON l.parser_name = p.name
        WHERE l.name = @linkName
        AND l.parser_name = @parserName
        AND p.type = @parserType
        """;

    private const string LinkNameParam = "@linkName";
    private const string ParserNameParam = "@parserName";
    private const string ParserTypeParam = "@parserType";
    private const string LinkNameColumn = "link_name";
    private const string ParserNameColumn = "parser_name";
    private const string ParserTypeColumn = "parser_type";
    private const string ParserStateColumn = "parser_state";
    private const string LinkUrlColumn = "link_url";

    public async Task<ParserLinkToRemove> Fetch(
        string linkName,
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = FetchSql;
        command.Parameters.Add(new NpgsqlParameter<string>(LinkNameParam, linkName));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserNameParam, parserName));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserTypeParam, parserType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new ParserLinkToRemoveNotFoundException(linkName, parserName, parserType);
        if (!await reader.ReadAsync(ct))
            throw new ParserLinkToRemoveNotFoundException(linkName, parserName, parserType);
        return new ParserLinkToRemove(
            reader.GetString(reader.GetOrdinal(LinkNameColumn)),
            reader.GetString(reader.GetOrdinal(ParserNameColumn)),
            reader.GetString(reader.GetOrdinal(ParserTypeColumn)),
            reader.GetString(reader.GetOrdinal(ParserStateColumn)),
            reader.GetString(reader.GetOrdinal(LinkUrlColumn))
        );
    }

    private const string SaveSql = """
        DELETE FROM scrapers_module.scraper_links
        WHERE name = @name AND parser_name = @parserName AND parser_type = @parserType AND url = @url;
        """;
    private const string SaveNameParam = "@name";
    private const string SaveUrlParam = "@url";

    public async Task<RemovedParserLink> Save(
        RemovedParserLink parserLink,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SaveSql;
        command.Parameters.Add(new NpgsqlParameter<string>(SaveNameParam, parserLink.Name));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserNameParam, parserLink.ParserName));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserTypeParam, parserLink.ParserType));
        command.Parameters.Add(new NpgsqlParameter<string>(SaveUrlParam, parserLink.Url));
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
