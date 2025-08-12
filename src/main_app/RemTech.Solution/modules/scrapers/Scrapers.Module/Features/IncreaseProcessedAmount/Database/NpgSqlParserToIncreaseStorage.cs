using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.IncreaseProcessedAmount.Exceptions;
using Scrapers.Module.Features.IncreaseProcessedAmount.Models;

namespace Scrapers.Module.Features.IncreaseProcessedAmount.Database;

internal sealed class NpgSqlParserToIncreaseStorage(NpgsqlDataSource dataSource)
    : IParserToIncreaseStorage
{
    private const string FetchSql = """
        SELECT 
            p.name as parser_name, 
            p.type as parser_type,
            l.name as link_name, 
            p.processed as parser_processed, 
            l.processed as link_processed
        FROM scrapers_module.scrapers p
        INNER JOIN scrapers_module.scraper_links l ON p.name = l.parser_name
        WHERE p.name = @parserName AND p.type = @parserType AND l.name = @linkName;
        """;

    private const string ParserNameParam = "@parserName";
    private const string ParserTypeParam = "@parserType";
    private const string LinkNameParam = "@linkName";
    private const string ParserNameColumn = "parser_name";
    private const string ParserTypeColumn = "parser_type";
    private const string LinkNameColumn = "link_name";
    private const string ParserProcessedColumn = "parser_processed";
    private const string LinkProcessedColumn = "link_processed";

    public async Task<ParserToIncreaseProcessed> Fetch(
        string parserName,
        string parserType,
        string linkName,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = FetchSql;
        command.Parameters.Add(new NpgsqlParameter<string>(ParserNameParam, parserName));
        command.Parameters.Add(new NpgsqlParameter<string>(ParserTypeParam, parserType));
        command.Parameters.Add(new NpgsqlParameter<string>(LinkNameParam, linkName));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new UnableToFindIncreaseProcessedParserException(
                parserName,
                parserType,
                linkName
            );
        if (!await reader.ReadAsync(ct))
            throw new UnableToFindIncreaseProcessedParserException(
                parserName,
                parserType,
                linkName
            );
        return new ParserToIncreaseProcessed(
            reader.GetString(reader.GetOrdinal(ParserNameColumn)),
            reader.GetString(reader.GetOrdinal(ParserTypeColumn)),
            reader.GetString(reader.GetOrdinal(LinkNameColumn)),
            reader.GetInt32(reader.GetOrdinal(ParserProcessedColumn)),
            reader.GetInt32(reader.GetOrdinal(LinkProcessedColumn))
        );
    }

    private const string SaveSql = """
        UPDATE scrapers_module.scrapers SET processed = @processed
        WHERE name = @name AND type = @type
        """;
    private const string NameParam = "@name";
    private const string TypeParam = "@type";
    private const string ProcessedParam = "@processed";

    public async Task<ParserWithIncreasedProcessed> Save(
        ParserWithIncreasedProcessed parser,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(ct);
        try
        {
            await using NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = SaveSql;
            command.Parameters.Add(new NpgsqlParameter<string>(NameParam, parser.ParserName));
            command.Parameters.Add(new NpgsqlParameter<string>(TypeParam, parser.ParserType));
            command.Parameters.Add(
                new NpgsqlParameter<int>(ProcessedParam, parser.ParserProcessed)
            );
            int affected = await command.ExecuteNonQueryAsync(ct);
            if (affected == 0)
                throw new UnableToFindIncreaseProcessedParserException(
                    parser.ParserName,
                    parser.ParserType,
                    parser.ParserLinkName
                );
            await SaveParserLink(parser, ct);
            await transaction.CommitAsync(ct);
            return parser;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private const string SaveParserLinkSql = """
        UPDATE scrapers_module.scraper_links SET processed = @processed
        WHERE name = @name AND parser_name = @parser_name AND parser_type = @parser_type
        """;
    private const string SaveParserLinkParserNameParam = "@parser_name";
    private const string SaveParserLinkParserTypeParam = "@parser_type";

    private async Task SaveParserLink(ParserWithIncreasedProcessed parser, CancellationToken ct)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SaveParserLinkSql;
        command.Parameters.Add(
            new NpgsqlParameter<string>(SaveParserLinkParserNameParam, parser.ParserName)
        );
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, parser.ParserLinkName));
        command.Parameters.Add(
            new NpgsqlParameter<string>(SaveParserLinkParserTypeParam, parser.ParserType)
        );
        command.Parameters.Add(new NpgsqlParameter<int>(ProcessedParam, parser.LinkProcessed));
        int affected = await command.ExecuteNonQueryAsync(ct);
        if (affected == 0)
            throw new UnableToFindIncreaseProcessedParserException(
                parser.ParserName,
                parser.ParserType,
                parser.ParserLinkName
            );
    }
}
