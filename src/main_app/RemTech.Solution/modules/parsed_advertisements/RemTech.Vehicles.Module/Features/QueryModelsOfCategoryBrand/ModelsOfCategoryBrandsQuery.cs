using Npgsql;

namespace RemTech.Vehicles.Module.Features.QueryModelsOfCategoryBrand;

internal sealed class ModelsOfCategoryBrandsQuery
{
    private const string Sql = """
        SELECT DISTINCT m.id as model_id, m.name as model_name
        FROM models_module.models m
        LEFT JOIN parsed_advertisements_module.parsed_vehicles v ON m.id = v.model_id
        WHERE v.kind_id = @categoryId AND v.brand_id = @brandId
        """;

    public async Task<IEnumerable<ModelsOfCategoryBrandsResult>> Query(
        Guid categoryId,
        Guid brandId,
        NpgsqlConnection connection,
        CancellationToken ct
    )
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@categoryId", categoryId));
        command.Parameters.Add(new NpgsqlParameter<Guid>("@brandId", brandId));
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return [];
        List<ModelsOfCategoryBrandsResult> results = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(0);
            string name = reader.GetString(1);
            results.Add(new ModelsOfCategoryBrandsResult(id, name));
        }
        return results;
    }
}
