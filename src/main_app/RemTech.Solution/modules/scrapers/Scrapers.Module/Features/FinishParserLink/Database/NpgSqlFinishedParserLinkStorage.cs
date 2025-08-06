using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.FinishParserLink.Exceptions;
using Scrapers.Module.Features.FinishParserLink.Models;

namespace Scrapers.Module.Features.FinishParserLink.Database;

internal sealed class NpgSqlFinishedParserLinkStorage(NpgsqlDataSource dataSource)
    : IFinishedParserLinkStorage
{
    public async Task<ParserLinkToFinish> Fetch(
        string parserName,
        string linkName,
        string parserType,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT p.state as parser_state, p.name as parser_name, p.type as parser_type, l.name as link_name
            FROM scrapers_module.scraper_links l
            INNER JOIN scrapers_module.scrapers p ON l.parser_name = p.name
            WHERE l.name = @linkName AND l.parser_name = @parserName AND p.type = @parserType; 
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@linkName", linkName));
        command.Parameters.Add(new NpgsqlParameter<string>("@parserName", parserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@parserType", parserType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new ParserLinkToFinishNotFoundException(parserName, linkName, parserType);
        if (!await reader.ReadAsync(ct))
            throw new ParserLinkToFinishNotFoundException(parserName, linkName, parserType);
        return new ParserLinkToFinish(
            reader.GetString(reader.GetOrdinal("parser_name")),
            reader.GetString(reader.GetOrdinal("parser_state")),
            reader.GetString(reader.GetOrdinal("parser_type")),
            reader.GetString(reader.GetOrdinal("link_name"))
        );
    }

    public async Task<FinishedParserLink> Save(
        FinishedParserLink link,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scraper_links
            SET total_seconds = @total,
                hours = @hours,
                minutes = @minutes,
                seconds = @seconds
            WHERE name = @name AND parser_name = @parserName;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<long>("@total", link.TotalElapsedSeconds));
        command.Parameters.Add(new NpgsqlParameter<int>("@minutes", link.Minutes));
        command.Parameters.Add(new NpgsqlParameter<int>("@hours", link.Hours));
        command.Parameters.Add(new NpgsqlParameter<int>("@seconds", link.Seconds));
        command.Parameters.Add(new NpgsqlParameter<string>("@name", link.LinkName));
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_name", link.ParserName));
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
