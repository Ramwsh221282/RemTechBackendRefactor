using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.CreateNewParserLink.Exceptions;
using Scrapers.Module.Features.CreateNewParserLink.Models;

namespace Scrapers.Module.Features.CreateNewParserLink.Database;

internal sealed class NpgSqlParsersWithLinkStorage(NpgsqlDataSource dataSource)
    : IParsersWithNewLinkStorage
{
    private const string FetchSql = """
        SELECT name, type, state, domain FROM scrapers_module.scrapers
        WHERE name = @name AND type = @type;
        """;
    private const string NameParam = "@name";
    private const string TypeParam = "@type";
    private const string NameColumn = "name";
    private const string TypeColumn = "type";
    private const string StateColumn = "state";
    private const string DomainColumn = "domain";

    public async Task<ParserWhereToPutLink> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = FetchSql;
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, parserName));
        command.Parameters.Add(new NpgsqlParameter<string>(TypeParam, parserType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new ParserWhereToPutLinkNotFoundException(parserName, parserType);
        if (!await reader.ReadAsync(ct))
            throw new ParserWhereToPutLinkNotFoundException(parserName, parserType);
        return new ParserWhereToPutLink(
            reader.GetString(reader.GetOrdinal(NameColumn)),
            reader.GetString(reader.GetOrdinal(TypeColumn)),
            reader.GetString(reader.GetOrdinal(StateColumn)),
            reader.GetString(reader.GetOrdinal(DomainColumn))
        );
    }

    private const string SaveSql = """
        INSERT INTO scrapers_module.scraper_links
        (name, parser_name, parser_type, url, activity, processed, total_seconds, hours, minutes, seconds)
        VALUES
        (@name, @parser_name, @parser_type, @url, @activity, @processed, @total_seconds, @hours, @minutes, @seconds);
        """;
    private const string ParserNameParam = "@parser_name";
    private const string UrlParam = "@url";
    private const string ActivityParam = "@activity";
    private const string ProcessedParam = "@processed";
    private const string TotalSecondsParam = "@total_seconds";
    private const string HoursParam = "@hours";
    private const string MinutesParam = "@minutes";
    private const string SecondsParam = "@seconds";
    private const string ParserTypeParam = "@parser_type";

    public async Task<ParserWithNewLink> Save(
        ParserWithNewLink parser,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SaveSql;
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, parser.Link.Name));
        command.Parameters.Add(
            new NpgsqlParameter<string>(ParserNameParam, parser.Link.ParserName)
        );
        command.Parameters.Add(new NpgsqlParameter<string>(UrlParam, parser.Link.Url));
        command.Parameters.Add(new NpgsqlParameter<bool>(ActivityParam, parser.Link.Active));
        command.Parameters.Add(
            new NpgsqlParameter<int>(ProcessedParam, parser.Link.Statistics.ParsedAmount)
        );
        command.Parameters.Add(
            new NpgsqlParameter<long>(TotalSecondsParam, parser.Link.Statistics.TotalElapsedSeconds)
        );
        command.Parameters.Add(
            new NpgsqlParameter<int>(HoursParam, parser.Link.Statistics.ElapsedHours)
        );
        command.Parameters.Add(
            new NpgsqlParameter<int>(MinutesParam, parser.Link.Statistics.ElapsedMinutes)
        );
        command.Parameters.Add(
            new NpgsqlParameter<int>(SecondsParam, parser.Link.Statistics.ElapsedSeconds)
        );
        command.Parameters.Add(
            new NpgsqlParameter<string>(ParserTypeParam, parser.Link.ParserType)
        );
        try
        {
            await command.ExecuteNonQueryAsync(ct);
            return parser;
        }
        catch (NpgsqlException ex)
        {
            if (
                ex.Message.Contains("scraper_links_pkey")
                || ex.Message.Contains("scraper_links_url_key")
            )
                throw new ParserLinkAlreadyExistsInParserException(parser.Parser, parser.Link);
            throw new Exception(ex.Message);
        }
    }
}
