using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.FinishParserLink.Exceptions;
using Scrapers.Module.Features.FinishParserLink.Models;

namespace Scrapers.Module.Features.FinishParserLink.Database;

internal sealed class NpgSqlFinishedParserLinkStorage(NpgsqlDataSource dataSource)
    : IFinishedParserLinkStorage
{
    private const string FetchSql = """
        SELECT p.state as parser_state, p.name as parser_name, p.type as parser_type, l.name as link_name
        FROM scrapers_module.scraper_links l
        INNER JOIN scrapers_module.scrapers p ON l.parser_name = p.name
        WHERE l.name = @linkName AND l.parser_name = @parserName AND p.type = @parserType; 
        """;
    private const string LinkNameParam = "@linkName";
    private const string ParserNameParam = "@parserName";
    private const string ParserTypeParam = "@parserType";
    private const string ParserNameColumn = "parser_name";
    private const string ParserTypeColumn = "parser_type";
    private const string LinkNameColumn = "link_name";

    public async Task<ParserLinkToFinish> Fetch(
        string parserName,
        string linkName,
        string parserType,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = FetchSql;
        command.Parameters.Add(new NpgsqlParameter<string>(LinkNameParam, linkName));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserNameParam, parserName));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserTypeParam, parserType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new ParserLinkToFinishNotFoundException(parserName, linkName, parserType);
        if (!await reader.ReadAsync(ct))
            throw new ParserLinkToFinishNotFoundException(parserName, linkName, parserType);
        return new ParserLinkToFinish(
            reader.GetString(reader.GetOrdinal(ParserNameColumn)),
            reader.GetString(reader.GetOrdinal(ParserTypeColumn)),
            reader.GetString(reader.GetOrdinal(LinkNameColumn))
        );
    }

    private const string SaveSql = """
        UPDATE scrapers_module.scraper_links
        SET total_seconds = @total,
            hours = @hours,
            minutes = @minutes,
            seconds = @seconds
        WHERE name = @name AND parser_name = @parser_name;
        """;
    private const string TotalParam = "@total";
    private const string HourParam = "@hours";
    private const string MinuteParam = "@minutes";
    private const string SecondParam = "@seconds";
    private const string NameParam = "@name";
    private const string ParserNameSaveParam = "@parser_name";

    public async Task<FinishedParserLink> Save(
        FinishedParserLink link,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SaveSql;
        command.Parameters.Add(new NpgsqlParameter<long>(TotalParam, link.TotalElapsedSeconds));
        command.Parameters.Add(new NpgsqlParameter<int>(MinuteParam, link.Minutes));
        command.Parameters.Add(new NpgsqlParameter<int>(HourParam, link.Hours));
        command.Parameters.Add(new NpgsqlParameter<int>(SecondParam, link.Seconds));
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, link.LinkName));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserNameSaveParam, link.ParserName));
        int affected = await command.ExecuteNonQueryAsync(ct);
        return affected == 0
            ? throw new ParserLinkToFinishNotFoundException(
                link.ParserName,
                link.LinkName,
                link.ParserType
            )
            : link;
    }
}
