using Npgsql;

namespace RemTech.Postgres.Adapter.Library.PgCommands;

public sealed class PgCommand(NpgsqlConnection connection, string sql)
{
    private readonly NpgsqlCommand _command = connection.CreateCommand();

    public NpgsqlCommand Command()
    {
        _command.CommandText = sql;
        return _command;
    }
}
