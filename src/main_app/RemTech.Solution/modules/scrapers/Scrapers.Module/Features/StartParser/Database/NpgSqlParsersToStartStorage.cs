using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.StartParser.Models;

namespace Scrapers.Module.Features.StartParser.Database;

internal sealed class NpgSqlParsersToStartStorage(NpgsqlDataSource dataSource)
    : IParsersToStartStorage
{
    private const string FetchSql = """
        SELECT 
            p.name as parser_name, 
            p.type as parser_type, 
            p.domain as parser_domain,
            l.name as link_name,
            l.parser_type as parser_link_type,
            l.parser_name as parser_link_name, 
            l.url as parser_link_url
        FROM scrapers_module.scrapers p
        LEFT JOIN scrapers_module.scraper_links l ON p.name = l.parser_name AND p.type = l.parser_type
        WHERE p.state = 'Ожидает' AND l.activity = TRUE AND next_run <= @next_run
        """;

    private const string NextRunParam = "@next_run";
    private const string ParserNameColumn = "parser_name";
    private const string LinkNameColumn = "link_name";
    private const string ParserLinkTypeColumn = "parser_link_type";
    private const string ParserLinkNameColumn = "parser_link_name";
    private const string ParserLinkUrlColumn = "parser_link_url";

    public async Task<IEnumerable<ParserToStart>> Fetch(
        DateTime nextRun,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = FetchSql;
        command.Parameters.Add(new NpgsqlParameter<DateTime>(NextRunParam, nextRun));
        Dictionary<string, ParserToStart> entries = [];
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return [];
        while (await reader.ReadAsync(ct))
        {
            string parserName = reader.GetString(reader.GetOrdinal(ParserNameColumn));
            if (!entries.TryGetValue(parserName, out ParserToStart? entry))
            {
                entry = ReadParserToStart(parserName, reader);
                entries.Add(parserName, entry);
            }

            if (await reader.IsDBNullAsync(reader.GetOrdinal(LinkNameColumn), ct))
                continue;
            string linkParserType = reader.GetString(reader.GetOrdinal(ParserLinkTypeColumn));
            string parserLinkName = reader.GetString(reader.GetOrdinal(ParserLinkNameColumn));
            if (entry.ParserName != parserLinkName && entry.ParserType != linkParserType)
                continue;
            string linkName = reader.GetString(reader.GetOrdinal(LinkNameColumn));
            string parserLinkUrl = reader.GetString(reader.GetOrdinal(ParserLinkUrlColumn));
            ParserLinksToStart linkToStart = new(
                linkName,
                parserLinkUrl,
                parserLinkName,
                linkParserType
            );
            entry.Links.Add(linkToStart);
        }
        return entries.Values;
    }

    private const string SaveSql = """
        UPDATE scrapers_module.scrapers SET 
        state = @state, 
        processed = @processed, 
        total_seconds = 0,
        hours = 0,
        minutes = 0,
        seconds = 0
        WHERE name = @name AND type = @type;
        """;
    private const string SaveNameParam = "@name";
    private const string SaveTypeParam = "@type";
    private const string SaveStateParam = "@state";
    private const string SaveProcessedParam = "@processed";

    public async Task<StartedParser> Save(StartedParser parser, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = new NpgsqlCommand(SaveSql, connection);
        await using NpgsqlTransaction transaction = await command.Connection!.BeginTransactionAsync(
            ct
        );
        try
        {
            command.CommandText = SaveSql;
            command.Parameters.Add(new NpgsqlParameter<string>(SaveNameParam, parser.ParserName));
            command.Parameters.Add(new NpgsqlParameter<string>(SaveTypeParam, parser.ParserType));
            command.Parameters.Add(new NpgsqlParameter<string>(SaveStateParam, parser.ParserState));
            command.Parameters.Add(new NpgsqlParameter<int>(SaveProcessedParam, parser.Processed));
            await command.ExecuteNonQueryAsync(ct);
            await SaveLinks(parser);
            await transaction.CommitAsync(ct);
            return parser;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private const string ParserNameParam = "@parser_name";
    private const string ParserTypeParam = "@parser_type";
    private const string ParserTypeColumn = "parser_type";
    private const string ParserDomainColumn = "parser_domain";

    private static ParserToStart ReadParserToStart(string name, DbDataReader reader)
    {
        return new ParserToStart(
            name,
            reader.GetString(reader.GetOrdinal(ParserTypeColumn)),
            reader.GetString(reader.GetOrdinal(ParserDomainColumn)),
            []
        );
    }

    private const string SaveLinksSql = """
        UPDATE scrapers_module.scraper_links SET 
        processed = @processed, 
        total_seconds = 0, 
        hours = 0, 
        minutes = 0, 
        seconds = 0
        WHERE parser_name = @parser_name AND parser_type = @parser_type AND activity = TRUE;
        """;

    private async Task SaveLinks(StartedParser parser)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(
            CancellationToken.None
        );
        await using NpgsqlCommand command = new NpgsqlCommand(SaveLinksSql, connection);
        command.CommandText = SaveLinksSql;
        command.Parameters.Add(new NpgsqlParameter<string>(ParserNameParam, parser.ParserName));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserTypeParam, parser.ParserType));
        await command.ExecuteNonQueryAsync(CancellationToken.None);
    }
}
