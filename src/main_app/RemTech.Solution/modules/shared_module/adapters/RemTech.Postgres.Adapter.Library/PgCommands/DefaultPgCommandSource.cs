using Npgsql;

namespace RemTech.Postgres.Adapter.Library.PgCommands;

public sealed class DefaultPgCommandSource : IPgCommandSource
{
    private readonly NpgsqlCommand _command;

    public DefaultPgCommandSource(NpgsqlCommand command)
    {
        _command = command;
    }
    
    public NpgsqlCommand Command() => _command;
}