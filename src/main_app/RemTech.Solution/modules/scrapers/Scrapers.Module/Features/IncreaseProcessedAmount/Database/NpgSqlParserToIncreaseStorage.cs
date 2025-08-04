using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.IncreaseProcessedAmount.Exceptions;
using Scrapers.Module.Features.IncreaseProcessedAmount.Models;

namespace Scrapers.Module.Features.IncreaseProcessedAmount.Database;

internal sealed class NpgSqlParserToIncreaseStorage(NpgsqlDataSource dataSource)
    : IParserToIncreaseStorage
{
    public async Task<ParserToIncreaseProcessed> Fetch(
        string parserName,
        string parserType,
        string linkName,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT 
                p.name as parser_name, 
                p.type as parser_type,
                p.state as parser_state,
                l.name as link_name, 
                p.processed as parser_processed, 
                l.processed as link_processed
            FROM scrapers_module.scrapers p
            INNER JOIN scrapers_module.scraper_links l ON p.name = l.parser_name
            WHERE p.name = @parserName AND p.type = @parserType AND l.name = @linkName;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@parserName", parserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@parserType", parserType));
        command.Parameters.Add(new NpgsqlParameter<string>("@linkName", linkName));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new UnableToFindIncreaseProcessedParserException(
                parserName,
                parserType,
                linkName
            );
        return new ParserToIncreaseProcessed(
            reader.GetString(reader.GetOrdinal("parser_name")),
            reader.GetString(reader.GetOrdinal("parser_type")),
            reader.GetString(reader.GetOrdinal("parser_state")),
            reader.GetString(reader.GetOrdinal("link_name")),
            reader.GetInt32(reader.GetOrdinal("parser_processed")),
            reader.GetInt32(reader.GetOrdinal("link_processed"))
        );
    }

    public async Task<ParserWithIncreasedProcessed> Save(
        ParserWithIncreasedProcessed parser,
        CancellationToken ct = default
    )
    {
        await SaveParser(parser, ct);
        await SaveParserLink(parser, ct);
        return parser;
    }

    private async Task SaveParser(ParserWithIncreasedProcessed parser, CancellationToken ct)
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scrapers SET processed = @processed
            WHERE name = @name AND type = @type
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parser.ParserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parser.ParserType));
        int affected = await command.ExecuteNonQueryAsync(ct);
        if (affected == 0)
            throw new UnableToFindIncreaseProcessedParserException(
                parser.ParserName,
                parser.ParserType,
                parser.ParserLinkName
            );
    }

    private async Task SaveParserLink(ParserWithIncreasedProcessed parser, CancellationToken ct)
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scraper_links SET processed = @processed
            WHERE name = @name AND parser_name = @parser_name
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_name", parser.ParserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parser.ParserLinkName));
        int affected = await command.ExecuteNonQueryAsync(ct);
        if (affected == 0)
            throw new UnableToFindIncreaseProcessedParserException(
                parser.ParserName,
                parser.ParserType,
                parser.ParserLinkName
            );
    }
}
