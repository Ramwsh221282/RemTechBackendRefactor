using Npgsql;
using Scrapers.Module.Features.CreateNewParser.Models;

namespace Scrapers.Module.Features.CreateNewParser.Database;

internal sealed class NpgSqlNewParsersStorage(NpgsqlDataSource dataSource) : INewParsersStorage
{
    public async Task Save(NewParser parser, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = string.Intern(
            """
            INSERT INTO scrapers_module.scrapers
            (name, type, state, domain, processed, total_seconds, hours, minutes, seconds, wait_days, next_run, last_run)
            VALUES
            (@name, @type, @state, @domain, @processed, @total_seconds, @hours, @minutes, @seconds, @wait_days, @next_run, @last_run)
            """
        );

        command.Parameters.Add(new NpgsqlParameter<string>("@name", parser.Name));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", parser.Type.Type));
        command.Parameters.Add(new NpgsqlParameter<string>("@state", parser.State.State));
        command.Parameters.Add(new NpgsqlParameter<string>("@domain", parser.Domain.Domain));
        command.Parameters.Add(
            new NpgsqlParameter<int>("@processed", parser.Statistics.ProcessedAmount)
        );
        command.Parameters.Add(
            new NpgsqlParameter<long>("@total_seconds", parser.Statistics.TotalElapsedSeconds)
        );
        command.Parameters.Add(new NpgsqlParameter<int>("@hours", parser.Statistics.ElapsedHours));
        command.Parameters.Add(
            new NpgsqlParameter<int>("@minutes", parser.Statistics.ElapsedMinutes)
        );
        command.Parameters.Add(
            new NpgsqlParameter<int>("@seconds", parser.Statistics.ElapsedSeconds)
        );
        command.Parameters.Add(new NpgsqlParameter<int>("@wait_days", parser.Schedule.WaitDays));
        command.Parameters.Add(new NpgsqlParameter<DateTime>("@next_run", parser.Schedule.NextRun));
        command.Parameters.Add(new NpgsqlParameter<DateTime>("@last_run", parser.Schedule.LastRun));
    }
}
