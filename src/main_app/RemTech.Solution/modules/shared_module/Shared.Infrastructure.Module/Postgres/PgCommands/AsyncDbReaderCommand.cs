using System.Data.Common;
using Npgsql;

namespace Shared.Infrastructure.Module.Postgres.PgCommands;

public sealed class AsyncDbReaderCommand(AsyncPreparedCommand prepared)
{
    public async Task<DbDataReader> AsyncReader(CancellationToken ct = default)
    {
        await using NpgsqlCommand command = await prepared.Command();
        return await command.ExecuteReaderAsync(ct);
    }
}
