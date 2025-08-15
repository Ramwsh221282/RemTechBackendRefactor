using System.Data.Common;
using Cleaners.Module.BackgroundJobs.StartingWaitingCleaner;
using Cleaners.Module.Domain;
using Npgsql;

namespace Cleaners.Module.Database;

internal sealed class ReadyToStartCleaners(NpgsqlConnection connection) : ICleaners
{
    private const string Sql = """
        SELECT
        id,
        cleaned_amount,
        last_run,
        next_run,
        wait_days,
        state,
        hours,
        minutes,
        seconds,
        items_date_day_threshold
        FROM cleaners_module.cleaners
        WHERE state = 'Ожидает' AND next_run <= @next_run;
        """;

    public async Task<ICleaner> Single(CancellationToken ct = default)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.Add(new NpgsqlParameter<DateTime>("@next_run", DateTime.UtcNow));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new NoCleanersToStartExistException();
        return Cleaner.Create(
            reader.GetGuid(0),
            reader.GetInt32(1),
            reader.GetDateTime(2),
            reader.GetDateTime(3),
            reader.GetInt32(4),
            reader.GetString(5),
            reader.GetInt32(6),
            reader.GetInt32(7),
            reader.GetInt32(8),
            reader.GetInt32(9)
        );
    }
}
