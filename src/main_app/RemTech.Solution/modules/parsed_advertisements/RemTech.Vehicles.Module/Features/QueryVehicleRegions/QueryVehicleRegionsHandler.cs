using System.Data.Common;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace RemTech.Vehicles.Module.Features.QueryVehicleRegions;

internal sealed class QueryVehicleRegionsHandler(NpgsqlDataSource dataSource)
    : ICommandHandler<QueryVehicleRegionsCommand, IEnumerable<QueryVehicleRegionsResult>>
{
    private const string Sql = """
        SELECT DISTINCT r.id, r.name, r.kind
        FROM parsed_advertisements_module.parsed_vehicles v
        LEFT JOIN locations_module.regions r ON r.id = v.geo_id
        """;

    public async Task<IEnumerable<QueryVehicleRegionsResult>> Handle(
        QueryVehicleRegionsCommand command,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return [];
        List<QueryVehicleRegionsResult> results = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(0);
            string name = reader.GetString(1);
            string kind = reader.GetString(2);
            results.Add(new QueryVehicleRegionsResult(id, name, kind));
        }
        return results;
    }
}
