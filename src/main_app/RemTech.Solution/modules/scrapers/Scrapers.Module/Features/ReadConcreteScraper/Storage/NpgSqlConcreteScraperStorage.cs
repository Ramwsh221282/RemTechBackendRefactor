using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;

namespace Scrapers.Module.Features.ReadConcreteScraper.Storage;

internal sealed class NpgSqlConcreteScraperStorage(NpgsqlDataSource dataSource)
    : IConcreteScraperStorage
{
    private const string Sql = """
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
        LEFT JOIN scrapers_module.scraper_links l ON l.parser_name = p.name AND l.parser_type = p.type
        WHERE p.name = @name AND p.type = @type;
        """;
    private const string NameParam = "@name";
    private const string TypeParam = "@type";
    private const string ParserNameColumn = "parser_name";
    private const string ParserTypeColumn = "parser_type";
    private const string ParserStateColumn = "parser_state";
    private const string ParserDomainColumn = "parser_domain";
    private const string ParserProcessedColumn = "parser_processed";
    private const string ParserTotalSecondsColumn = "parser_total_seconds";
    private const string ParserHoursColumn = "parser_hours";
    private const string ParserMinutesColumn = "parser_minutes";
    private const string ParserSecondsColumn = "parser_seconds";
    private const string ParserWaitDaysColumn = "parser_wait_days";
    private const string ParserNextRunColumn = "parser_next_run";
    private const string ParserLastRunColumn = "parser_last_run";
    private const string LinkNameColumn = "link_name";
    private const string LinkParserNameColumn = "link_parser_name";
    private const string LinkParserTypeColumn = "link_parser_type";
    private const string LinkUrlColumn = "link_url";
    private const string LinkActivityColumn = "link_activity";
    private const string LinkProcessedColumn = "link_processed";
    private const string LinkTotalSecondsColumn = "link_total_seconds";
    private const string LinkHoursColumn = "link_hours";
    private const string LinkMinutesColumn = "link_minutes";
    private const string LinkSecondsColumn = "link_seconds";

    public async Task<ParserResult?> Read(string name, string type, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.Add(new NpgsqlParameter(NameParam, name));
        command.Parameters.Add(new NpgsqlParameter(TypeParam, type));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return null;
        Dictionary<string, ParserResult> entries = [];
        while (await reader.ReadAsync(ct))
        {
            string parserName = reader.GetString(reader.GetOrdinal(ParserNameColumn));
            if (!entries.TryGetValue(parserName, out ParserResult? parserResult))
            {
                parserResult = new ParserResult(
                    parserName,
                    reader.GetString(reader.GetOrdinal(ParserTypeColumn)),
                    reader.GetString(reader.GetOrdinal(ParserStateColumn)),
                    reader.GetString(reader.GetOrdinal(ParserDomainColumn)),
                    reader.GetInt32(reader.GetOrdinal(ParserProcessedColumn)),
                    reader.GetInt64(reader.GetOrdinal(ParserTotalSecondsColumn)),
                    reader.GetInt32(reader.GetOrdinal(ParserHoursColumn)),
                    reader.GetInt32(reader.GetOrdinal(ParserMinutesColumn)),
                    reader.GetInt32(reader.GetOrdinal(ParserSecondsColumn)),
                    reader.GetInt32(reader.GetOrdinal(ParserWaitDaysColumn)),
                    reader.GetDateTime(reader.GetOrdinal(ParserLastRunColumn)),
                    reader.GetDateTime(reader.GetOrdinal(ParserNextRunColumn)),
                    []
                );
                entries.Add(parserName, parserResult);
            }

            if (await reader.IsDBNullAsync(reader.GetOrdinal(LinkNameColumn), ct))
                continue;
            string linkName = reader.GetString(reader.GetOrdinal(LinkNameColumn));
            string linkParserName = reader.GetString(reader.GetOrdinal(LinkParserNameColumn));
            string linkParserType = reader.GetString(reader.GetOrdinal(LinkParserTypeColumn));
            if (parserResult.Name != linkParserName || parserResult.Type != linkParserType)
                continue;
            ParserLinkResult parserLink = new ParserLinkResult(
                linkName,
                linkParserName,
                linkParserType,
                reader.GetString(reader.GetOrdinal(LinkUrlColumn)),
                reader.GetBoolean(reader.GetOrdinal(LinkActivityColumn)),
                reader.GetInt32(reader.GetOrdinal(LinkProcessedColumn)),
                reader.GetInt64(reader.GetOrdinal(LinkTotalSecondsColumn)),
                reader.GetInt32(reader.GetOrdinal(LinkHoursColumn)),
                reader.GetInt32(reader.GetOrdinal(LinkMinutesColumn)),
                reader.GetInt32(reader.GetOrdinal(LinkSecondsColumn))
            );
            entries[parserName].Links.Add(parserLink);
        }
        return entries.TryGetValue(name, out ParserResult? value) ? value : null;
    }
}
