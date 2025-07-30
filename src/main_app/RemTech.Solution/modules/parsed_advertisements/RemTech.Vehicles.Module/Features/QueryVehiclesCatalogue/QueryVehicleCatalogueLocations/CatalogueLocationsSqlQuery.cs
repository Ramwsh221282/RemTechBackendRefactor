using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Database.SqlStringGeneration;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations;

public sealed class CatalogueLocationsSqlQuery : IVehiclesSqlQuery
{
    private readonly List<string> _filters = [];
    private readonly List<NpgsqlParameter> _parameters = [];
    private static readonly string _sql = string.Intern(
        """
        SELECT DISTINCT
        g.id as location_id,
        g.text as location_name,
        g.kind as location_kind
        FROM parsed_advertisements_module.parsed_vehicles v        
        INNER JOIN parsed_advertisements_module.vehicle_kinds k ON k.id = v.kind_id
        INNER JOIN parsed_advertisements_module.vehicle_brands b ON b.id = v.brand_id
        INNER JOIN parsed_advertisements_module.vehicle_models m ON m.id = v.model_id
        INNER JOIN parsed_advertisements_module.geos g on v.geo_id = g.id
        INNER JOIN parsed_advertisements_module.parsed_vehicle_characteristics pvc ON v.id = pvc.vehicle_id
        INNER JOIN parsed_advertisements_module.vehicle_characteristics vc ON pvc.ctx_id = vc.id
        """
    );

    public void AcceptFilter(string filter, NpgsqlParameter parameter)
    {
        _filters.Add(filter);
        _parameters.Add(parameter);
    }

    public void AcceptFilter(string filter, IEnumerable<NpgsqlParameter> parameters)
    {
        _filters.Add(filter);
        _parameters.AddRange(parameters);
    }

    public CatalogueLocationsSqlQuery ApplyRequest(VehiclesQueryRequest request)
    {
        CompositeVehicleSpeicification speicification = new();
        speicification = request.ApplyTo(speicification);
        speicification.ApplyTo(this);
        return this;
    }

    public IPgCommandSource PrepareCommand(NpgsqlConnection connection)
    {
        SqlGenerated generated = SqlGenerator
            .SourceSql(_sql)
            .ApplyFilters(new SqlFilters(_filters))();
        NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = generated.Sql;
        foreach (NpgsqlParameter parameter in _parameters)
            command.Parameters.Add(parameter);
        return new DefaultPgCommandSource(command);
    }

    public void AcceptAscending(string orderingField) { }

    public void AcceptPagination(int page) { }

    public void AcceptDescending(string orderingField) { }
}
