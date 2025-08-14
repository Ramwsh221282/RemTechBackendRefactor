using System.Data.Common;
using Cleaners.Module.Domain;
using Cleaners.Module.Domain.Exceptions;
using Npgsql;

namespace Cleaners.Module.Database;

internal sealed class NpgSqlCleaners(NpgsqlConnection connection) : ICleaners
{
    private const string Sql =
        "SELECT id, cleaned_amount, last_run, next_run, wait_days, state, hours, minutes, seconds FROM cleaners_module.cleaners";

    public async Task<ICleaner> Single(CancellationToken ct = default)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new CleanerDoesNotExistsException();
        return Cleaner.Create(
            reader.GetGuid(0),
            reader.GetInt32(1),
            reader.GetDateTime(2),
            reader.GetDateTime(3),
            reader.GetInt32(4),
            reader.GetString(5),
            reader.GetInt32(6),
            reader.GetInt32(7),
            reader.GetInt32(8)
        );
    }
}
