using System.Data.Common;
using System.Text;
using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;
using RemTech.Vehicles.Module.Types.Transport;
using Serilog;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles;

public sealed class VehiclesSqlQuery(ILogger logger) : IVehiclesSqlQuery
{
    private const int MaxPageSize = 20;
    private readonly List<string> _filters = [];
    private readonly List<NpgsqlParameter> _parameters = [];
    private string _ordering = string.Empty;
    private string _pagination = string.Empty;

    private readonly string _sql = string.Intern(
        """
        SELECT
        v.id as vehicle_id,
        v.price as vehicle_price,
        v.photos as vehicle_photos,
        v.is_nds as vehicle_nds,
        k.id as kind_id,
        k.text as kind_name,
        b.id as brand_id,
        b.text as brand_name,
        m.id as model_id,
        m.text as model_name,
        g.id as geo_id,
        g.text as geo_text,
        g.kind as geo_kind,
        vc.text as ctx_name,
        vc.id as ctx_id,
        pvc.ctx_value as ctx_value,
        pvc.ctx_measure as ctx_measure
        FROM parsed_advertisements_module.parsed_vehicles v        
        INNER JOIN parsed_advertisements_module.vehicle_kinds k ON k.id = v.kind_id
        INNER JOIN parsed_advertisements_module.vehicle_brands b ON b.id = v.brand_id
        INNER JOIN parsed_advertisements_module.vehicle_models m ON m.id = v.model_id
        INNER JOIN parsed_advertisements_module.geos g on v.geo_id = g.id
        INNER JOIN parsed_advertisements_module.parsed_vehicle_characteristics pvc ON v.id = pvc.vehicle_id
        INNER JOIN parsed_advertisements_module.vehicle_characteristics vc ON pvc.ctx_id = vc.id
        """
    );

    public VehiclesSqlQuery ApplyRequest(VehiclesQueryRequest request)
    {
        CompositeVehicleSpeicification speicification = new();
        speicification = request.ApplyTo(speicification);
        speicification.ApplyTo(this);
        return this;
    }

    public async Task<IEnumerable<Vehicle>> Retrieve(
        NpgsqlConnection connection,
        CancellationToken ct = default
    )
    {
        IPgCommandSource prepared = PrepareCommand(connection);
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(prepared)
        ).AsyncReader(ct);
        return await new SqlParsedVehicles(reader).Read(ct);
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

    public void AcceptAscending(string orderingField)
    {
        _ordering = $" ORDER BY {orderingField} ASC ";
    }

    public void AcceptDescending(string orderingField)
    {
        _ordering = $" ORDER BY {orderingField} DESC ";
    }

    public IPgCommandSource PrepareCommand(NpgsqlConnection connection)
    {
        NpgsqlCommand command = connection.CreateCommand();
        string sql = GenerateSql();
        logger.Information("Generated sql:");
        logger.Information(sql);
        command.CommandText = sql;
        foreach (NpgsqlParameter parameter in _parameters)
            command.Parameters.Add(parameter);
        logger.Information("Sql parameters:");
        foreach (NpgsqlParameter parameter in command.Parameters)
        {
            string name = parameter.ParameterName;
            string value = parameter.Value!.ToString()!;
            logger.Information("Parameter name: {Name}. Parameter value: {Value}.", name, value);
        }
        return new DefaultPgCommandSource(command);
    }

    public void AcceptPagination(int page)
    {
        if (page <= 0)
        {
            _pagination = " LIMIT 0 ";
            return;
        }

        int offset = (page - 1) * MaxPageSize;
        _pagination = " LIMIT @limit OFFSET @offset ";
        _parameters.Add(new NpgsqlParameter<int>("@limit", MaxPageSize));
        _parameters.Add(new NpgsqlParameter<int>("@offset", offset));
    }

    private string GenerateSql()
    {
        StringBuilder sb = new StringBuilder(_sql);
        sb = sb.AppendLine(GenerateFilters());
        if (!string.IsNullOrWhiteSpace(_ordering))
            sb = sb.AppendLine(_ordering);
        if (!string.IsNullOrWhiteSpace(_pagination))
            sb = sb.AppendLine(_pagination);
        return sb.ToString();
    }

    private string GenerateFilters()
    {
        if (_filters.Count == 0)
            return string.Empty;
        StringBuilder stringBuilder = new();
        string joined = string.Join(" AND ", _filters);
        return stringBuilder.AppendLine(" WHERE 1=1 AND ").AppendLine(joined).ToString();
    }
}
