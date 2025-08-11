using Npgsql;

namespace Shared.Infrastructure.Module.Postgres.PgCommands;

public sealed class PreparedPgCommand(ParametrizingPgCommand command)
{
    private readonly NpgsqlCommand _command = command.Command();

    public NpgsqlCommand Command()
    {
        _command.Prepare();
        return _command;
    }
}
