using Npgsql;

namespace Shared.Infrastructure.Module.Postgres.PgCommands;

public sealed class PgCommand(NpgsqlConnection connection, string sql)
{
    private readonly NpgsqlCommand _command = connection.CreateCommand();

    public NpgsqlCommand Command()
    {
        _command.CommandText = sql;
        return _command;
    }
}
