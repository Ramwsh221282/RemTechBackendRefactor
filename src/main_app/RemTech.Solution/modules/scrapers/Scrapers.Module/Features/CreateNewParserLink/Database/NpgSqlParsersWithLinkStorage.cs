using Npgsql;
using Scrapers.Module.Features.CreateNewParserLink.Exceptions;
using Scrapers.Module.Features.CreateNewParserLink.Models;

namespace Scrapers.Module.Features.CreateNewParserLink.Database;

internal sealed class NpgSqlParsersWithLinkStorage(NpgsqlDataSource dataSource)
    : IParsersWithNewLinkStorage
{
    public async Task<ParserWithNewLink> Save(
        ParserWithNewLink parser,
        CancellationToken ct = default
    )
    {
        if (!await EnsureParserExists(parser, ct))
            throw new ParserWhereToPutLinkNotFoundException(parser.Parser.Name, parser.Parser.Type);
        string sql = string.Intern(
            """
            INSERT INTO scrapers_module.scraper_links
            (name, parser_name, url, activity, processed, total_seconds, hours, minutes, seconds)
            VALUES
            (@name, @parser_name, @url, @activity, @processed, @total_seconds, @hours, @minutes, @seconds)
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
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new ParserLinkAlreadyExistsInParserException(parser.Parser, parser.Link)
            : parser;
    }

    private async Task<bool> EnsureParserExists(
        ParserWithNewLink parser,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT 1 FROM scrapers_module.scrapers
            WHERE name = @name AND type = @type;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parser.Parser.Name));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parser.Parser.Type));
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 1;
    }
}
