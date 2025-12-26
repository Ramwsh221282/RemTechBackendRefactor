using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Cqrs;

namespace RemTech.Vehicles.Module.Features.QueryVehicleModels;

internal sealed class QueryVehicleModelsHandler(NpgsqlDataSource dataSource)
    : ICommandHandler<QueryVehicleModelsCommand, IEnumerable<QueryVehicleModelsResult>>
{
    private const string Sql = """
        SELECT DISTINCT m.id, m.name
        FROM parsed_advertisements_module.parsed_vehicles v
        LEFT JOIN models_module.models m ON m.id = v.model_id
        """;

    public async Task<IEnumerable<QueryVehicleModelsResult>> Handle(
        QueryVehicleModelsCommand command,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            return [];
        List<QueryVehicleModelsResult> results = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(0);
            string name = reader.GetString(1);
            results.Add(new QueryVehicleModelsResult(id, name));
        }
        return results;
    }
}
