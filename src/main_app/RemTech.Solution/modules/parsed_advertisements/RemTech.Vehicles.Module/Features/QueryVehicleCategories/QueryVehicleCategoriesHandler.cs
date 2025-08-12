using System.Data.Common;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace RemTech.Vehicles.Module.Features.QueryVehicleCategories;

internal sealed class QueryVehicleCategoriesHandler(NpgsqlDataSource dataSource)
    : ICommandHandler<QueryVehicleCategoriesCommand, IEnumerable<VehicleCategoriesResult>>
{
    private const string Sql = """
        SELECT DISTINCT c.id, c.name
        FROM parsed_advertisements_module.parsed_vehicles v
        LEFT JOIN categories_module.categories c ON c.id = v.kind_id
        """;

    public async Task<IEnumerable<VehicleCategoriesResult>> Handle(
        QueryVehicleCategoriesCommand command,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return [];
        List<VehicleCategoriesResult> results = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(0);
            string name = reader.GetString(1);
            results.Add(new VehicleCategoriesResult(id, name));
        }
        return results;
    }
}
