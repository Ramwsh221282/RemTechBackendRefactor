using System.Data.Common;
using Npgsql;
using Pgvector;
using RemTech.Vehicles.Module.Types.Kinds.ValueObjects;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.Types.Kinds.Storage;

internal sealed class VectorKindsStorage(NpgsqlDataSource dataSource, IEmbeddingGenerator generator)
    : IVehicleKindsStorage
{
    public async Task<VehicleKind> Store(VehicleKind kind)
    {
        string sql = string.Intern(
            """
            SELECT id, text FROM parsed_advertisements_module.vehicle_kinds ORDER BY embedding <=> @embedding LIMIT 1;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        string name = kind.Name();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@embedding", new Vector(generator.Generate(name)));
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            throw new UnableToStoreVehicleKindException(
                "Невозможно получить тип техники по tgrm запросу.",
                name
            );
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string text = reader.GetString(reader.GetOrdinal("text"));
        VehicleKindIdentity identity = new VehicleKindIdentity(
            new VehicleKindId(id),
            new VehicleKindText(text)
        );
        return kind.ChangeIdentity(identity);
    }
}
