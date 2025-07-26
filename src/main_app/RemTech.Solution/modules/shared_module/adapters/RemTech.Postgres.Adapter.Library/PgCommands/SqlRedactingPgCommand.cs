using Npgsql;

namespace RemTech.Postgres.Adapter.Library.PgCommands;

public sealed class SqlRedactingPgCommand : IPgCommandSource
{
    private readonly NpgsqlCommand _origin;
    public NpgsqlCommand Command()
    {
        return _origin;
    }

    public SqlRedactingPgCommand(PgCommand command)
    {
        _origin = command.Command();
    }

    public SqlRedactingPgCommand(ParametrizingPgCommand parametrizingPgCommand)
    {
        _origin = parametrizingPgCommand.Command();
    }

    public SqlRedactingPgCommand Redacted(string otherQuery)
    {
        _origin.CommandText = otherQuery;
        return this;
    }
}