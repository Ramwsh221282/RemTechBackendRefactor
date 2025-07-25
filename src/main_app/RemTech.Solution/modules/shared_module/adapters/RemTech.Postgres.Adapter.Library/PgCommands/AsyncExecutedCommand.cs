using Npgsql;

namespace RemTech.Postgres.Adapter.Library.PgCommands;

public sealed class AsyncExecutedCommand(AsyncPreparedCommand prepared)
{
    public async Task<int> AsyncExecuted(CancellationToken ct = default)
    {
        await using NpgsqlCommand command = await prepared.Command();
        return await command.ExecuteNonQueryAsync(ct);
    }
}
