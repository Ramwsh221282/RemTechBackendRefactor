using Npgsql;

namespace Shared.Infrastructure.Module.Postgres.PgCommands;

public sealed class AsyncExecutedCommand(AsyncPreparedCommand prepared)
{
    public async Task<int> AsyncExecuted(CancellationToken ct = default)
    {
        await using NpgsqlCommand command = await prepared.Command();
        return await command.ExecuteNonQueryAsync(ct);
    }
}
