using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.InstantlyEnableParser.Exceptions;
using Scrapers.Module.Features.InstantlyEnableParser.Models;

namespace Scrapers.Module.Features.InstantlyEnableParser.Storage;

internal sealed class NpgSqlInstantlyEnabledParsersStorage(NpgsqlDataSource dataSource)
    : IInstantlyEnabledParsersStorage
{
    private const string FetchSql = """
        SELECT 
            p.name as parser_name, 
            p.type as parser_type, 
            p.domain as parser_domain,
            p.state as parser_state,
            l.name as link_name,
            l.parser_type as parser_link_type,
            l.parser_name as parser_link_name, 
            l.url as parser_link_url
        FROM scrapers_module.scrapers p
        LEFT JOIN scrapers_module.scraper_links l ON p.name = l.parser_name AND p.type = l.parser_type
        WHERE p.name = @parser_name AND p.type = @parser_type AND l.activity = TRUE;
        """;
    private const string ParserNameParam = "@parser_name";
    private const string ParserTypeParam = "@parser_type";
    private const string ParserNameColumn = "parser_name";
    private const string ParserTypeColumn = "parser_type";
    private const string ParserStateColumn = "parser_state";
    private const string ParserDomainColumn = "parser_domain";
    private const string LinkNameColumn = "link_name";
    private const string ParserLinkUrlColumn = "parser_link_url";
    private const string ParserLinkTypeColumn = "parser_link_type";
    private const string ParserLinkNameColumn = "parser_link_name";

    public async Task<ParserToInstantlyEnable> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = FetchSql;
        command.Parameters.Add(new NpgsqlParameter<string>(ParserNameParam, parserName));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserTypeParam, parserType));
        Dictionary<string, ParserToInstantlyEnable> entries = [];
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new ParserToInstantlyEnableNotFoundException(parserName, parserType);
        while (await reader.ReadAsync(ct))
        {
            string parserToStartName = reader.GetString(reader.GetOrdinal(ParserNameColumn));
            if (!entries.TryGetValue(parserName, out ParserToInstantlyEnable? entry))
            {
                entry = new ParserToInstantlyEnable(
                    parserToStartName,
                    reader.GetString(reader.GetOrdinal(ParserTypeColumn)),
                    reader.GetString(reader.GetOrdinal(ParserStateColumn)),
                    reader.GetString(reader.GetOrdinal(ParserDomainColumn))
                );
                entries.Add(parserName, entry);
            }

            if (await reader.IsDBNullAsync(reader.GetOrdinal(LinkNameColumn), ct))
                continue;
            string linkName = reader.GetString(reader.GetOrdinal(LinkNameColumn));
            string parserLinkUrl = reader.GetString(reader.GetOrdinal(ParserLinkUrlColumn));
            string linkParserType = reader.GetString(reader.GetOrdinal(ParserLinkTypeColumn));
            string parserLinkName = reader.GetString(reader.GetOrdinal(ParserLinkNameColumn));
            entry.AddLink(linkName, parserLinkUrl, parserLinkName, linkParserType);
        }
        return entries[parserName];
    }

    private const string SaveSql = """
        UPDATE scrapers_module.scrapers
        SET 
        state = @state, 
        next_run = @next_run, 
        last_run = @last_run,
        total_seconds = 0,
        hours = 0,
        minutes = 0,
        seconds = 0,
        processed = 0
        WHERE name = @name AND type = @type;
        """;

    private const string SaveNameParam = "@name";
    private const string SaveTypeParam = "@type";
    private const string SaveStateParam = "@state";
    private const string SaveNextRunParam = "@next_run";
    private const string SaveLastRunParam = "@last_run";

    public async Task Save(
        string parserName,
        string parserType,
        string state,
        DateTime nextRun,
        DateTime lastRun,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(ct);
        try
        {
            await using NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = SaveSql;
            command.Parameters.Add(new NpgsqlParameter<string>(SaveNameParam, parserName));
            command.Parameters.Add(new NpgsqlParameter<string>(SaveTypeParam, parserType));
            command.Parameters.Add(new NpgsqlParameter<string>(SaveStateParam, state));
            command.Parameters.Add(new NpgsqlParameter<DateTime>(SaveNextRunParam, nextRun));
            command.Parameters.Add(new NpgsqlParameter<DateTime>(SaveLastRunParam, lastRun));
            await command.ExecuteNonQueryAsync(ct);
            await SaveLinks(parserName, parserType, connection);
            await transaction.CommitAsync(ct);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private const string SaveLinksSql = """
        UPDATE scrapers_module.scraper_links
        SET 
            total_seconds = 0,
            hours = 0,
            minutes = 0,
            seconds = 0,
            processed = 0
        WHERE parser_name = @parser_name AND parser_type = @parser_type AND activity = TRUE;
        """;
    private const string SaveLinkParserNameParam = "@parser_name";
    private const string SaveLinkParserTypeParam = "@parser_type";

    private async Task SaveLinks(string parserName, string parserType, NpgsqlConnection connection)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SaveLinksSql;
        command.Parameters.Add(new NpgsqlParameter<string>(SaveLinkParserNameParam, parserName));
        command.Parameters.Add(new NpgsqlParameter<string>(SaveLinkParserTypeParam, parserType));
        await command.ExecuteNonQueryAsync();
    }
}
