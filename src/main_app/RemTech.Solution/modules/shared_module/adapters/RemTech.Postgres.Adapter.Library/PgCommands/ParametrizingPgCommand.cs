using Npgsql;

namespace RemTech.Postgres.Adapter.Library.PgCommands;

public sealed class ParametrizingPgCommand(PgCommand command)
{
    private readonly NpgsqlCommand _origin = command.Command();

    public NpgsqlCommand Command()
    {
        return _origin;
    }

    public ParametrizingPgCommand With<T>(string name, T value)
    {
        _origin.Parameters.Add(new NpgsqlParameter<T>(name, value));
        return this;
    }

    public ParametrizingPgCommand WithIf<T>(string name, T value, Func<T, bool> predicate)
    {
        return !predicate(value) ? this : With(name, value);
    }
}
