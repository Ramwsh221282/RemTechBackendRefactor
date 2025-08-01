using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Database.SqlStringGeneration;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary;

public sealed class VehicleCharacteristicsDictionarySqlQuery : IVehiclesSqlQuery
{
    private readonly List<string> _filters = [];
    private readonly List<NpgsqlParameter> _parameters = [];

    private readonly string _sql = string.Intern(
        """
        SELECT
        pvc.ctx_id as ctx_id,
        pvc.ctx_name as ctx_text,
        pvc.ctx_measure as ctx_measure,
        pvc.ctx_value as ctx_value
        FROM parsed_advertisements_module.parsed_vehicles v     
        INNER JOIN parsed_advertisements_module.parsed_vehicle_characteristics pvc ON v.id = pvc.vehicle_id
        """
    );

    public VehicleCharacteristicsDictionarySqlQuery ApplyRequest(VehiclesQueryRequest request)
    {
        CompositeVehicleSpeicification speicification = new();
        speicification = request.ApplyTo(speicification);
        speicification.ApplyTo(this);
        return this;
    }

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

    public IPgCommandSource PrepareCommand(NpgsqlConnection connection)
    {
        NpgsqlCommand command = connection.CreateCommand();
        SqlGeneration generation = SqlGenerator
            .SourceSql(_sql)
            .ApplyFilters(new SqlFilters(_filters));
        SqlGenerated generated = generation();
        command.CommandText = generated.Sql;
        foreach (NpgsqlParameter parameter in _parameters)
            command.Parameters.Add(parameter);
        return new DefaultPgCommandSource(command);
    }

    public void AcceptAscending(string orderingField) { }

    public void AcceptDescending(string orderingField) { }

    public void AcceptPagination(int page) { }
}
