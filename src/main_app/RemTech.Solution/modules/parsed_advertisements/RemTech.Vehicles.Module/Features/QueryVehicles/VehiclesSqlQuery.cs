using System.Data.Common;
using System.Text;
using Npgsql;
using Pgvector;
using RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;
using RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;
using Serilog;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using Shared.Infrastructure.Module.Postgres.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryVehicles;

internal sealed class VehiclesSqlQuery(ILogger logger, IEmbeddingGenerator generator)
    : IVehiclesSqlQuery
{
    private const int MaxPageSize = 20;
    private readonly List<string> _filters = [];
    private readonly List<NpgsqlParameter> _parameters = [];
    private readonly VehiclesSqlQueryOrdering _ordering = new();
    private Vector? _vector = null;
    private string _pagination = string.Empty;

    private readonly string _sql = string.Intern(
        """
        SELECT
        v.id as vehicle_id,
        v.price as vehicle_price,
        v.is_nds as vehicle_nds,
        v.brand_id as brand_id,
        v.kind_id as kind_id,
        v.model_id as model_id,
        v.geo_id as geo_id,
        v.object as object_data
        FROM parsed_advertisements_module.parsed_vehicles v
        """
    );

    public VehiclesSqlQuery ApplyRequest(VehiclesQueryRequest request)
    {
        CompositeVehicleSpeicification speicification = new();
        speicification = request.ApplyTo(speicification);
        speicification.ApplyTo(this);
        return this;
    }

    public async Task<IEnumerable<VehiclePresentation>> Retrieve(
        NpgsqlConnection connection,
        CancellationToken ct = default
    )
    {
        IPgCommandSource prepared = PrepareCommand(connection);
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(prepared)
        ).AsyncReader(ct);
        return await new VehiclePresentsReader(reader).Read(ct);
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
        _ordering.WithOrdering(orderingField, "ASC");
    }

    public void AcceptDescending(string orderingField)
    {
        _ordering.WithOrdering(orderingField, "DESC");
    }

    public void AcceptTextSearch(string textSearch)
    {
        if (!string.IsNullOrWhiteSpace(textSearch))
        {
            logger.Information("Applying text search.");
            float[] embeddings = generator.Generate(textSearch);
            _vector = new Vector(embeddings);
            _ordering.WithVectorSearch();
            logger.Information("Text search applied.");
        }
    }

    public IPgCommandSource PrepareCommand(NpgsqlConnection connection)
    {
        NpgsqlCommand command = connection.CreateCommand();
        string sql = GenerateSql();
        logger.Information("Generated sql:");
        logger.Information("{Sql}", sql);
        command.CommandText = sql;
        logger.Information("Sql parameters:");
        foreach (NpgsqlParameter parameter in _parameters)
        {
            command.Parameters.Add(parameter);
            string name = parameter.ParameterName;
            string value = parameter.Value!.ToString()!;
            logger.Information("Parameter name: {Name}. Parameter value: {Value}.", name, value);
        }
        if (_vector != null)
            command.Parameters.AddWithValue("@embedding", _vector);
        return new DefaultPgCommandSource(command);
    }

    public void AcceptPagination(int page)
    {
        int offset = (page - 1) * MaxPageSize;
        _pagination = " LIMIT @limit OFFSET @offset ";
        _parameters.Add(new NpgsqlParameter<int>("@limit", MaxPageSize));
        _parameters.Add(new NpgsqlParameter<int>("@offset", offset));
    }

    private string GenerateSql()
    {
        StringBuilder sb = new StringBuilder(_sql);
        sb = sb.AppendLine(GenerateFilters());
        logger.Information("Applying text search sql part.");
        sb = _ordering.Apply(sb);
        if (!string.IsNullOrWhiteSpace(_pagination))
            sb = sb.AppendLine(_pagination);
        logger.Information("Text search sql part applied.");
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
