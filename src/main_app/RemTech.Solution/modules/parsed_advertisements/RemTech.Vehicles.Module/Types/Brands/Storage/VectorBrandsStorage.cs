using System.Data.Common;
using Npgsql;
using Pgvector;
using RemTech.Vehicles.Module.Types.Brands.ValueObjects;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.Types.Brands.Storage;

internal sealed class VectorBrandsStorage(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator
) : IVehicleBrandsStorage
{
    public async Task<VehicleBrand> Store(VehicleBrand brand)
    {
        string sql = string.Intern(
            """
            SELECT id, text FROM parsed_advertisements_module.vehicle_brands
            ORDER BY embedding <=> @embedding LIMIT 1;
            """
        );
        string parameter = brand.Name();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new Vector(generator.Generate(parameter)));
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            throw new UnableToStoreBrandException(
                "Не удается получить бренд по векторному запросу.",
                brand.Name()
            );
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string text = reader.GetString(reader.GetOrdinal("text"));
        VehicleBrandIdentity otherIdentity = new VehicleBrandIdentity(
            new VehicleBrandId(id),
            new VehicleBrandText(text)
        );
        return brand.ChangeIdentity(otherIdentity);
    }
}
