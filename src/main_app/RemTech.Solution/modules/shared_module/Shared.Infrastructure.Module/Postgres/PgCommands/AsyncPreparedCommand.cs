using Npgsql;

namespace Shared.Infrastructure.Module.Postgres.PgCommands;

public sealed class AsyncPreparedCommand(IPgCommandSource command)
{
    private readonly NpgsqlCommand _command = command.Command();
    private bool _prepared;

    public async Task<NpgsqlCommand> Command()
    {
        if (_prepared)
            return _command;
        await _command.PrepareAsync();
        _prepared = true;
        return _command;
    }
}
