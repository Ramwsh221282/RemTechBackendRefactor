using System.Text.Json;
using Npgsql;
using NpgsqlTypes;
using RemTech.Spares.Module.Features.SinkSpare.Persistance;

namespace RemTech.Spares.Module.Features.SinkSpare.Json;

internal sealed class SpareJsonObject
    : ISparePersistanceCommandSource<NpgsqlCommand, SpareSqlPersistanceCommand>
{
    private readonly Dictionary<string, object> _objectProperties = [];

    public void Add(string name, object value)
    {
        _objectProperties.Add(name, value);
    }

    public SpareSqlPersistanceCommand Modify(SpareSqlPersistanceCommand persistance)
    {
        NpgsqlCommand command = persistance.Read();
        string json = JsonSerializer.Serialize(_objectProperties);
        NpgsqlParameter<string> parameter = new NpgsqlParameter<string>("@object", json)
        {
            NpgsqlDbType = NpgsqlDbType.Jsonb,
        };
        command.Parameters.Add(parameter);
        return new SpareSqlPersistanceCommand(command);
    }
}
