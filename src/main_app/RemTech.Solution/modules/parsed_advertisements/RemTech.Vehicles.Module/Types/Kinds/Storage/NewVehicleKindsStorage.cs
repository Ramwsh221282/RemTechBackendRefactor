using Npgsql;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.Types.Kinds.Storage;

internal sealed class NewVehicleKindsStorage(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator
) : IVehicleKindsStorage
{
    public async Task<VehicleKind> Store(VehicleKind kind)
    {
        string name = kind.Name();
        Guid id = kind.Id();
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.vehicle_kinds(id, text, embedding)
            VALUES (@id, @text, @embedding)
            ON CONFLICT(text) DO NOTHING;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", id));
        command.Parameters.Add(new NpgsqlParameter<string>("@text", name));
        command.Parameters.AddWithValue("@embedding", generator.Generate(name));
        int affected = await command.ExecuteNonQueryAsync();
        return affected == 0
            ? throw new UnableToStoreVehicleKindException(
                $"Не удается сохранить тип техники. Дубликат по названию.",
                name
            )
            : kind;
    }
}
