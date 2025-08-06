using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;

namespace Scrapers.Module.Features.ReadAllTransportParsers.Storage;

internal sealed class NpgSqlAllTransportParsersStorage(NpgsqlDataSource dataSource)
    : IAllTransportParsersStorage
{
    public async Task<IEnumerable<ParserResult>> Read(CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            SELECT 
            p.name as parser_name, 
            p.type as parser_type, 
            p.state as parser_state, 
            p.domain as parser_domain, 
            p.processed as parser_processed, 
            p.total_seconds as parser_total_seconds, 
            p.hours as parser_hours,
            p.minutes as parser_minutes,
            p.seconds as parser_seconds,
            p.wait_days as parser_wait_days,
            p.next_run as parser_next_run,
            p.last_run as parser_last_run,
            l.name as link_name,
            l.parser_name as link_parser_name,
            l.parser_type as link_parser_type,
            l.url as link_url,
            l.activity as link_activity,
            l.processed as link_processed,
            l.total_seconds as link_total_seconds,
            l.hours as link_hours,
            l.minutes as link_minutes,
            l.seconds as link_seconds
            FROM scrapers_module.scrapers p
            INNER JOIN scrapers_module.scraper_links l ON l.parser_name = p.name
            ORDER BY p.name ASC;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        command.CommandText = sql;
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            return [];
        Dictionary<string, ParserResult> entries = [];
        while (await reader.ReadAsync(ct))
        {
            string parserName = reader.GetString(reader.GetOrdinal("parser_name"));
            if (!entries.TryGetValue(parserName, out ParserResult? parserResult))
            {
                parserResult = new ParserResult(
                    parserName,
                    reader.GetString(reader.GetOrdinal("parser_type")),
                    reader.GetString(reader.GetOrdinal("parser_state")),
                    reader.GetString(reader.GetOrdinal("parser_domain")),
                    reader.GetInt32(reader.GetOrdinal("parser_processed")),
                    reader.GetInt64(reader.GetOrdinal("parser_total_seconds")),
                    reader.GetInt32(reader.GetOrdinal("parser_hours")),
                    reader.GetInt32(reader.GetOrdinal("parser_minutes")),
                    reader.GetInt32(reader.GetOrdinal("parser_seconds")),
                    reader.GetInt32(reader.GetOrdinal("parser_wait_days")),
                    reader.GetDateTime(reader.GetOrdinal("parser_last_run")),
                    reader.GetDateTime(reader.GetOrdinal("parser_next_run")),
                    []
                );
                entries.Add(parserName, parserResult);
            }

            string linkName = reader.GetString(reader.GetOrdinal("link_name"));
            string linkParserName = reader.GetString(reader.GetOrdinal("link_parser_name"));
            string linkParserType = reader.GetString(reader.GetOrdinal("link_parser_type"));
            if (parserResult.Name == linkParserName && parserResult.Type == linkParserType)
            {
                ParserLinkResult parserLink = new ParserLinkResult(
                    linkName,
                    linkParserName,
                    linkParserType,
                    reader.GetString(reader.GetOrdinal("link_url")),
                    reader.GetBoolean(reader.GetOrdinal("link_activity")),
                    reader.GetInt32(reader.GetOrdinal("link_processed")),
                    reader.GetInt64(reader.GetOrdinal("link_total_seconds")),
                    reader.GetInt32(reader.GetOrdinal("link_hours")),
                    reader.GetInt32(reader.GetOrdinal("link_minutes")),
                    reader.GetInt32(reader.GetOrdinal("link_seconds"))
                );
                entries[parserName].Links.Add(parserLink);
            }
        }
        return entries.Values;
    }
}
