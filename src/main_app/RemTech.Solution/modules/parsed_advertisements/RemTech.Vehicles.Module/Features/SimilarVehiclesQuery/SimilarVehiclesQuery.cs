using System.Data.Common;
using Npgsql;
using Pgvector;
using RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.Features.SimilarVehiclesQuery;

internal sealed class SimilarVehiclesQuery
{
    private const string Sql = """
        SELECT DISTINCT
            vehicle_id,
            vehicle_price,
            vehicle_nds as vehicle_nds,
            brand_id,
            category_id,
            model_id,
            region_id,
            vehicle_source_url,
            vehicle_object_data,
            vehicle_description
        FROM (
            SELECT 
                v.id as vehicle_id,
                v.price as vehicle_price,
                v.is_nds as vehicle_nds,
                v.brand_id as brand_id,
                v.kind_id as category_id,
                v.model_id as model_id,
                v.geo_id as region_id,
                v.source_url as vehicle_source_url,
                v.object as vehicle_object_data,
                v.description as vehicle_description,
                v.embedding
            FROM parsed_advertisements_module.parsed_vehicles v
            WHERE v.id != @id
            ORDER BY v.embedding <=> @embedding
            LIMIT 5
        ) AS subquery;
        """;

    public async Task<IEnumerable<VehiclePresentation>> Query(
        string id,
        string text,
        NpgsqlConnection connection,
        IEmbeddingGenerator generator,
        CancellationToken ct = default
    )
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(id))
            return [];
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        Vector vector = new(generator.Generate(text));
        command.Parameters.AddWithValue("@embedding", vector);
        command.Parameters.AddWithValue("@id", id);
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return [];
        List<VehiclePresentation> result = [];
        while (await reader.ReadAsync(ct))
            result.Add(VehiclePresentation.FromReader(reader));
        return result;
    }
}
