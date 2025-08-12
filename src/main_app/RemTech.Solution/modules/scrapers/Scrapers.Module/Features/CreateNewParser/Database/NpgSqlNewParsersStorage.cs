using System.Data.Common;
using Npgsql;
using Scrapers.Module.Features.CreateNewParser.Exceptions;
using Scrapers.Module.Features.CreateNewParser.Models;

namespace Scrapers.Module.Features.CreateNewParser.Database;

internal sealed class NpgSqlNewParsersStorage(NpgsqlDataSource dataSource) : INewParsersStorage
{
    private const string SaveSql = """
        INSERT INTO scrapers_module.scrapers
        (name, type, state, domain, processed, total_seconds, hours, minutes, seconds, wait_days, next_run, last_run)
        VALUES
        (@name, @type, @state, @domain, @processed, @total_seconds, @hours, @minutes, @seconds, @wait_days, @next_run, @last_run)
        """;

    private const string NameParam = "@name";
    private const string TypeParam = "@type";
    private const string StateParam = "@state";
    private const string DomainParam = "@domain";
    private const string ProcessedParam = "@processed";
    private const string TotalSecondsParam = "@total_seconds";
    private const string HourParam = "@hours";
    private const string MinutesParam = "@minutes";
    private const string SecondsParam = "@seconds";
    private const string WaitDaysParam = "@wait_days";
    private const string NextRunParam = "@next_run";
    private const string LastRunParam = "@last_run";

    public async Task Save(NewParser parser, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await ValidateDuplication(connection, parser);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SaveSql;

        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, parser.Name));
        command.Parameters.Add(new NpgsqlParameter<string>(TypeParam, parser.Type.Type));
        command.Parameters.Add(new NpgsqlParameter<string>(StateParam, parser.State.State));
        command.Parameters.Add(new NpgsqlParameter<string>(DomainParam, parser.Domain.Domain));
        command.Parameters.Add(
            new NpgsqlParameter<int>(ProcessedParam, parser.Statistics.ProcessedAmount)
        );
        command.Parameters.Add(
            new NpgsqlParameter<long>(TotalSecondsParam, parser.Statistics.TotalElapsedSeconds)
        );
        command.Parameters.Add(new NpgsqlParameter<int>(HourParam, parser.Statistics.ElapsedHours));
        command.Parameters.Add(
            new NpgsqlParameter<int>(MinutesParam, parser.Statistics.ElapsedMinutes)
        );
        command.Parameters.Add(
            new NpgsqlParameter<int>(SecondsParam, parser.Statistics.ElapsedSeconds)
        );
        command.Parameters.Add(new NpgsqlParameter<int>(WaitDaysParam, parser.Schedule.WaitDays));
        command.Parameters.Add(
            new NpgsqlParameter<DateTime>(NextRunParam, parser.Schedule.NextRun)
        );
        command.Parameters.Add(
            new NpgsqlParameter<DateTime>(LastRunParam, parser.Schedule.LastRun)
        );
        try
        {
            await command.ExecuteNonQueryAsync(ct);
        }
        catch (NpgsqlException ex)
        {
            if (ex.Message.Contains("scrapers_name_type_key"))
                throw new ParserNameAndTypeDuplicateException(parser.Name, parser.Type.Type);
        }
    }

    private const string ValidateDuplicationSql =
        "SELECT COUNT(name) as amount FROM scrapers_module.scrapers WHERE name = @name AND type = @type AND domain = @domain";
    private const string AmountColumn = "amount";

    private async Task ValidateDuplication(NpgsqlConnection connection, NewParser parser)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = ValidateDuplicationSql;
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, parser.Name));
        command.Parameters.Add(new NpgsqlParameter<string>(TypeParam, parser.Type.Type));
        command.Parameters.Add(new NpgsqlParameter<string>(DomainParam, parser.Domain.Domain));
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        long amount = reader.GetInt64(reader.GetOrdinal(AmountColumn));
        if (amount > 0)
            throw new ParserNameAndTypeDuplicateException(parser.Name, parser.Type.Type);
    }
}
