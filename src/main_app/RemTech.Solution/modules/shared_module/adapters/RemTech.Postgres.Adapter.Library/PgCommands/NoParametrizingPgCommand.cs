using Npgsql;

namespace RemTech.Postgres.Adapter.Library.PgCommands;

public sealed class NoParametrizingPgCommand(PgCommand command) : IPgCommandSource 
{
    private readonly NpgsqlCommand _origin = command.Command();
    public NpgsqlCommand Command()
    {
        return _origin;
    }
}