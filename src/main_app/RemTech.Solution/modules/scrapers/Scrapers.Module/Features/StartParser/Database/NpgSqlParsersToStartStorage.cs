using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.StartParser.Models;

namespace Scrapers.Module.Features.StartParser.Database;

internal sealed class NpgSqlParsersToStartStorage(NpgsqlDataSource dataSource)
    : IParsersToStartStorage
{
    public async Task<IEnumerable<ParserToStart>> Fetch(CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            SELECT 
                p.name as parser_name, 
                p.type as parser_type, 
                p.state as parser_state, 
                l.name as link_name, 
                l.parser_name as parser_link_name, 
                l.url as parser_link_url
            FROM scrapers_module.scrapers p
            INNER JOIN scrapers_module.scraper_links l ON p.name = l.parser_name
            WHERE p.state = 'Ожидает' AND l.activity = TRUE              
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        command.CommandText = sql;
        Dictionary<string, ParserToStart> entries = [];
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return [];
        while (await reader.ReadAsync(ct))
        {
            string parserName = reader.GetString(reader.GetOrdinal("parser_name"));
            if (!entries.TryGetValue(parserName, out ParserToStart? entry))
            {
                entry = new ParserToStart(
                    reader.GetString(reader.GetOrdinal("parser_name")),
                    reader.GetString(reader.GetOrdinal("parser_type")),
                    reader.GetString(reader.GetOrdinal("parser_state")),
                    []
                );
                entries.Add(parserName, entry);
            }

            string linkName = reader.GetString(reader.GetOrdinal("link_name"));
            string parserLinkName = reader.GetString(reader.GetOrdinal("parser_link_name"));
            string parserLinkUrl = reader.GetString(reader.GetOrdinal("parser_link_url"));
            ParserLinksToStart linkToStart = new(linkName, parserLinkUrl, parserLinkName);
            if (linkToStart.LinkParserName == entry.ParserName)
                entry.Links.Add(linkToStart);
        }

        return entries.Values;
    }

    public async Task<StartedParser> Start(StartedParser parser, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scrapers SET state = @state
            WHERE name = @name AND type = @type;
            """
        );
        NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parser.ParserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parser.ParserType));
        await command.ExecuteNonQueryAsync(ct);
        return parser;
    }
}
