using Npgsql;

namespace RemTech.Postgres.Adapter.Library.PgCommands;

public sealed class AsyncPreparedCommand(ParametrizingPgCommand command)
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
