using Npgsql;

namespace Shared.Infrastructure.Module.Postgres.PgCommands;

public sealed class ExecutedPgCommand(PreparedPgCommand command)
{
    private readonly NpgsqlCommand _command = command.Command();

    public void Executed()
    {
        _command.ExecuteNonQuery();
        _command.Dispose();
    }
}
