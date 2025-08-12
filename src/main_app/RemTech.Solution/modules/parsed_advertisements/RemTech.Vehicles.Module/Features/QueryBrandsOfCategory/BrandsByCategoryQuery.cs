using Npgsql;

namespace RemTech.Vehicles.Module.Features.QueryBrandsOfCategory;

internal sealed class BrandsByCategoryQuery
{
    private const string Sql = """
        SELECT DISTINCT b.name as brand_name, b.id as brand_id FROM brands_module.brands b
        LEFT JOIN parsed_advertisements_module.parsed_vehicles v ON b.id = v.brand_id
        WHERE v.kind_id = @categoryId;
        """;

    public async Task<IEnumerable<BrandsByCategoryResult>> Query(
        Guid categoryId,
        NpgsqlConnection connection,
        CancellationToken ct = default
    )
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@categoryId", categoryId));
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return [];
        List<BrandsByCategoryResult> brands = [];
        while (await reader.ReadAsync(ct))
        {
            string name = reader.GetString(0);
            Guid id = reader.GetGuid(1);
            brands.Add(new BrandsByCategoryResult(id, name));
        }

        return brands;
    }
}
