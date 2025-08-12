using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.ChangeLinkActivity.Exceptions;
using Scrapers.Module.Features.ChangeLinkActivity.Models;

namespace Scrapers.Module.Features.ChangeLinkActivity.Database;

internal sealed class NpgSqlLinkActivityToChangeStorage(NpgsqlDataSource dataSource)
    : ILinkActivityToChangeStorage
{
    private const string FetchSql = """
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
        """;

    private const string NameParam = "@name";
    private const string ParserNameParam = "@parser_name";
    private const string TypeParam = "@type";
    private const string LinkNameColumn = "link_name";
    private const string LinkParserNameColumn = "link_parser_name";
    private const string LinkActivityColumn = "link_activity";
    private const string ParserTypeColumn = "parser_type";
    private const string ParserStateColumn = "parser_state";

    public async Task<LinkActivityToChange> Fetch(
        string name,
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using var command = connection.CreateCommand();
        command.CommandText = FetchSql;
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, name));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserNameParam, parserName));
        command.Parameters.Add(new NpgsqlParameter<string>(TypeParam, parserType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new LinkActivityToChangeNotFoundException(name, parserName, parserType);
        if (!await reader.ReadAsync(ct))
            throw new LinkActivityToChangeNotFoundException(name, parserName, parserType);
        string linkName = reader.GetString(reader.GetOrdinal(LinkNameColumn));
        string parserLinkName = reader.GetString(reader.GetOrdinal(LinkParserNameColumn));
        bool activity = reader.GetBoolean(reader.GetOrdinal(LinkActivityColumn));
        string linkParserType = reader.GetString(reader.GetOrdinal(ParserTypeColumn));
        string linkParserState = reader.GetString(reader.GetOrdinal(ParserStateColumn));
        LinkActivityToChange link = new(
            linkName,
            parserLinkName,
            linkParserType,
            activity,
            linkParserState
        );
        return link;
    }

    private const string SaveSql = """
        UPDATE scrapers_module.scraper_links
        SET activity = @activity
        WHERE name = @name AND parser_name = @parser_name;
        """;
    private const string ActivityParam = "@activity";

    public async Task<LinkWithChangedActivity> Save(
        LinkWithChangedActivity link,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SaveSql;
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, link.Name));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserNameParam, link.ParserName));
        command.Parameters.Add(new NpgsqlParameter<bool>(ActivityParam, link.CurrentActivity));
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
