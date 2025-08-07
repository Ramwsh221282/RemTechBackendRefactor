using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.StartParser.Models;

namespace Scrapers.Module.Features.StartParser.Database;

internal sealed class NpgSqlParsersToStartStorage(NpgsqlDataSource dataSource)
    : IParsersToStartStorage
{
    public async Task<IEnumerable<ParserToStart>> Fetch(
        DateTime nextRun,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
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
            WHERE p.state = 'Ожидает' AND l.activity = TRUE AND next_run < @next_run
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<DateTime>("@next_run", nextRun));
        Dictionary<string, ParserToStart> entries = [];
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return [];
        while (await reader.ReadAsync(ct))
        {
            string parserName = reader.GetString(reader.GetOrdinal("parser_name"));
            if (!entries.TryGetValue(parserName, out ParserToStart? entry))
            {
                entry = ReadParserToStart(parserName, reader);
                entries.Add(parserName, entry);
            }

            if (await reader.IsDBNullAsync(reader.GetOrdinal("link_name"), ct))
                continue;
            string linkParserType = reader.GetString(reader.GetOrdinal("parser_link_type"));
            string parserLinkName = reader.GetString(reader.GetOrdinal("parser_link_name"));
            if (entry.ParserName != parserLinkName && entry.ParserType != linkParserType)
                continue;
            string linkName = reader.GetString(reader.GetOrdinal("link_name"));
            string parserLinkUrl = reader.GetString(reader.GetOrdinal("parser_link_url"));
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

    public async Task<StartedParser> Save(StartedParser parser, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scrapers SET 
            state = @state, 
            processed = @processed, 
            total_seconds = 0,
            hours = 0,
            minutes = 0,
            seconds = 0
            WHERE name = @name AND type = @type;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        await using NpgsqlTransaction transaction = await command.Connection!.BeginTransactionAsync(
            ct
        );
        try
        {
            command.CommandText = sql;
            command.Parameters.Add(new NpgsqlParameter<string>("@name", parser.ParserName));
            command.Parameters.Add(new NpgsqlParameter<string>("@type", parser.ParserType));
            command.Parameters.Add(new NpgsqlParameter<string>("@state", parser.ParserState));
            command.Parameters.Add(new NpgsqlParameter<int>("@processed", parser.Processed));
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

    private static ParserToStart ReadParserToStart(string name, DbDataReader reader)
    {
        return new ParserToStart(
            name,
            reader.GetString(reader.GetOrdinal("parser_type")),
            reader.GetString(reader.GetOrdinal("parser_domain")),
            []
        );
    }

    private async Task SaveLinks(StartedParser parser)
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scraper_links SET 
            processed = @processed, 
            total_seconds = 0, 
            hours = 0, 
            minutes = 0, 
            seconds = 0
            WHERE parser_name = @parser_name AND parser_type = @parser_type AND activity = TRUE;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(
            CancellationToken.None
        );
        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_name", parser.ParserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_type", parser.ParserType));
        await command.ExecuteNonQueryAsync(CancellationToken.None);
    }
}
