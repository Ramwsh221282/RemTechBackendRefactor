using System.Data.Common;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace RemTech.Vehicles.Module.Features.QueryVehicleBrands;

internal sealed class QueryVehicleBrandsHandler(NpgsqlDataSource dataSource)
    : ICommandHandler<QueryVehicleBrandsCommand, IEnumerable<QueryVehicleBrandsResult>>
{
    private const string Sql = """
        SELECT DISTINCT b.id, b.name
        FROM parsed_advertisements_module.parsed_vehicles v
        LEFT JOIN brands_module.brands b ON b.id = v.brand_id
        """;

    public async Task<IEnumerable<QueryVehicleBrandsResult>> Handle(
        QueryVehicleBrandsCommand command,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return [];
        List<QueryVehicleBrandsResult> results = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(0);
            string name = reader.GetString(1);
            results.Add(new QueryVehicleBrandsResult(id, name));
        }
        return results;
    }
}
