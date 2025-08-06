using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.CreateNewParserLink.Exceptions;
using Scrapers.Module.Features.CreateNewParserLink.Models;

namespace Scrapers.Module.Features.CreateNewParserLink.Database;

internal sealed class NpgSqlParsersWithLinkStorage(NpgsqlDataSource dataSource)
    : IParsersWithNewLinkStorage
{
    public async Task<ParserWhereToPutLink> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT name, type, state, domain FROM scrapers_module.scrapers
            WHERE name = @name AND type = @type;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parserType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        return !await reader.ReadAsync(ct)
            ? throw new ParserWhereToPutLinkNotFoundException(parserName, parserType)
            : new ParserWhereToPutLink(
                reader.GetString(reader.GetOrdinal("name")),
                reader.GetString(reader.GetOrdinal("type")),
                reader.GetString(reader.GetOrdinal("state")),
                reader.GetString(reader.GetOrdinal("domain"))
            );
    }

    public async Task<ParserWithNewLink> Save(
        ParserWithNewLink parser,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            INSERT INTO scrapers_module.scraper_links
            (name, parser_name, parser_type, url, activity, processed, total_seconds, hours, minutes, seconds)
            VALUES
            (@name, @parser_name, @parser_type, @url, @activity, @processed, @total_seconds, @hours, @minutes, @seconds)
            ON CONFLICT (name, parser_name) DO NOTHING;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parser.Link.Name));
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_name", parser.Link.ParserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@url", parser.Link.Url));
        command.Parameters.Add(new NpgsqlParameter<bool>("@activity", parser.Link.Active));
        command.Parameters.Add(
            new NpgsqlParameter<int>("@processed", parser.Link.Statistics.ParsedAmount)
        );
        command.Parameters.Add(
            new NpgsqlParameter<long>("@total_seconds", parser.Link.Statistics.TotalElapsedSeconds)
        );
        command.Parameters.Add(
            new NpgsqlParameter<int>("@hours", parser.Link.Statistics.ElapsedHours)
        );
        command.Parameters.Add(
            new NpgsqlParameter<int>("@minutes", parser.Link.Statistics.ElapsedMinutes)
        );
        command.Parameters.Add(
            new NpgsqlParameter<int>("@seconds", parser.Link.Statistics.ElapsedSeconds)
        );
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_type", parser.Link.ParserType));
        try
        {
            await command.ExecuteNonQueryAsync(ct);
            return parser;
        }
        catch (NpgsqlException ex)
        {
            if (ex.Message.Contains("scraper_links_url_key"))
                throw new ParserLinkAlreadyExistsInParserException(parser.Parser, parser.Link);
            throw new Exception("Invalid error resolve.");
        }
    }
}
