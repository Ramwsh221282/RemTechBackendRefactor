using System.Data.Common;
using Npgsql;

namespace RemTech.Postgres.Adapter.Library.PgCommands;

public sealed class AsyncDbReaderCommand(AsyncPreparedCommand prepared)
{
    public async Task<DbDataReader> AsyncReader(CancellationToken ct = default)
    {
        await using NpgsqlCommand command = await prepared.Command();
        return await command.ExecuteReaderAsync(ct);
    }
}
