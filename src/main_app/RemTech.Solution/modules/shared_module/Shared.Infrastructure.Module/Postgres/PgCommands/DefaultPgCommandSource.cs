using Npgsql;

namespace Shared.Infrastructure.Module.Postgres.PgCommands;

public sealed class DefaultPgCommandSource : IPgCommandSource
{
    private readonly NpgsqlCommand _command;

    public DefaultPgCommandSource(NpgsqlCommand command)
    {
        _command = command;
    }

    public NpgsqlCommand Command() => _command;
}
