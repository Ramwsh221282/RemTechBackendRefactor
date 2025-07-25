using Npgsql;
using NpgsqlTypes;

namespace RemTech.Postgres.Adapter.Library.PgCommands;

public sealed class ParametrizingPgCommand(PgCommand command)
{
    private readonly NpgsqlCommand _origin = command.Command();
    private int _parametersProvided = 0;

    public NpgsqlCommand Command()
    {
        if (_parametersProvided == 0)
            throw new InvalidOperationException("No sql parameters provided.");
        return _origin;
    }

    public ParametrizingPgCommand With<T>(string name, T value)
    {
        _origin.Parameters.Add(new NpgsqlParameter<T>(name, value));
        _parametersProvided++;
        return this;
    }
    
    public ParametrizingPgCommand With<T>(string name, T value, NpgsqlDbType type)
    {
        NpgsqlParameter<T> parameter = new(name, value);
        parameter.NpgsqlDbType = type;
        _origin.Parameters.Add(parameter);
        _parametersProvided++;
        return this;
    }

    public ParametrizingPgCommand WithIf<T>(string name, T value, Func<T, bool> predicate)
    {
        return !predicate(value) ? this : With(name, value);
    }
}
