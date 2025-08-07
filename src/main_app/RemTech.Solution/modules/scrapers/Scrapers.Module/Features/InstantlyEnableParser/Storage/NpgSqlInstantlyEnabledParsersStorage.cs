using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.InstantlyEnableParser.Exceptions;
using Scrapers.Module.Features.InstantlyEnableParser.Models;

namespace Scrapers.Module.Features.InstantlyEnableParser.Storage;

internal sealed class NpgSqlInstantlyEnabledParsersStorage(NpgsqlDataSource dataSource)
    : IInstantlyEnabledParsersStorage
{
    public async Task<ParserToInstantlyEnable> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT 
                p.name as parser_name, 
                p.type as parser_type, 
                p.domain as parser_domain,
                p.state as parser_state,
                l.name as link_name,
                l.parser_type as parser_link_type,
                l.parser_name as parser_link_name, 
                l.url as parser_link_url
            FROM scrapers_module.scrapers p
            LEFT JOIN scrapers_module.scraper_links l ON p.name = l.parser_name AND p.type = l.parser_type
            WHERE p.name = @parser_name AND p.type = @parser_type AND l.activity = TRUE;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_name", parserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_type", parserType));
        Dictionary<string, ParserToInstantlyEnable> entries = [];
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new ParserToInstantlyEnableNotFoundException(parserName, parserType);
        while (await reader.ReadAsync(ct))
        {
            string parserToStartName = reader.GetString(reader.GetOrdinal("parser_name"));
            if (!entries.TryGetValue(parserName, out ParserToInstantlyEnable? entry))
            {
                entry = new ParserToInstantlyEnable(
                    parserToStartName,
                    reader.GetString(reader.GetOrdinal("parser_type")),
                    reader.GetString(reader.GetOrdinal("parser_state")),
                    reader.GetString(reader.GetOrdinal("parser_domain"))
                );
                entries.Add(parserName, entry);
            }

            if (await reader.IsDBNullAsync(reader.GetOrdinal("link_name"), ct))
                continue;
            string linkName = reader.GetString(reader.GetOrdinal("link_name"));
            string parserLinkUrl = reader.GetString(reader.GetOrdinal("parser_link_url"));
            string linkParserType = reader.GetString(reader.GetOrdinal("parser_link_type"));
            string parserLinkName = reader.GetString(reader.GetOrdinal("parser_link_name"));
            entry.AddLink(linkName, parserLinkUrl, parserLinkName, linkParserType);
        }
        return entries[parserName];
    }

    public async Task Save(
        string parserName,
        string parserType,
        string state,
        DateTime nextRun,
        DateTime lastRun,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scrapers
            SET 
            state = @state, 
            next_run = @next_run, 
            last_run = @last_run,
            total_seconds = 0,
            hours = 0,
            minutes = 0,
            seconds = 0,
            processed = 0
            WHERE name = @name AND type = @type;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(ct);
        try
        {
            await using NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.Add(new NpgsqlParameter<string>("@name", parserName));
            command.Parameters.Add(new NpgsqlParameter<string>("@type", parserType));
            command.Parameters.Add(new NpgsqlParameter<string>("@state", state));
            command.Parameters.Add(new NpgsqlParameter<DateTime>("@next_run", nextRun));
            command.Parameters.Add(new NpgsqlParameter<DateTime>("@last_run", lastRun));
            await command.ExecuteNonQueryAsync(ct);
            await SaveLinks(parserName, parserType, connection);
            await transaction.CommitAsync(ct);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private async Task SaveLinks(string parserName, string parserType, NpgsqlConnection connection)
    {
        string sql = string.Intern(
            """
            UPDATE scrapers_module.scraper_links
            SET 
                total_seconds = 0,
                hours = 0,
                minutes = 0,
                seconds = 0,
                processed = 0
            WHERE parser_name = @parser_name AND parser_type = @parser_type AND activity = TRUE;
            """
        );
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_name", parserName));
        command.Parameters.Add(new NpgsqlParameter<string>("@parser_type", parserType));
        await command.ExecuteNonQueryAsync();
    }
}
