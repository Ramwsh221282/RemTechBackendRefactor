using Npgsql;

namespace RemTech.Postgres.Adapter.Library.PgCommands;

public sealed class ExecutedPgCommand(PreparedPgCommand command)
{
    private readonly NpgsqlCommand _command = command.Command();

    public void Executed()
    {
        _command.ExecuteNonQuery();
        _command.Dispose();
    }
}
