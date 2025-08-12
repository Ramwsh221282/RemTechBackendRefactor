using System.Data.Common;
using System.Text;
using Npgsql;
using Pgvector;
using RemTech.Vehicles.Module.Features.QueryVehicles;
using RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using Shared.Infrastructure.Module.Postgres.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesAmount;

internal sealed class VehiclesAmountSqlQuery(IEmbeddingGenerator generator) : IVehiclesSqlQuery
{
    private readonly List<string> _filters = [];
    private readonly List<NpgsqlParameter> _parameters = [];
    private readonly VehiclesSqlQueryOrdering _ordering = new();
    private Vector? _vector = null;

    private readonly string _sql = string.Intern(
        """
        SELECT
        COUNT(v.id) as AMOUNT
        FROM parsed_advertisements_module.parsed_vehicles v
        """
    );

    public VehiclesAmountSqlQuery ApplyRequest(VehiclesAmountRequest request)
    {
        CompositeVehicleSpeicification speicification = new();
        speicification = request.ApplyTo(speicification);
        speicification.ApplyTo(this);
        return this;
    }

    public async Task<long> Retrieve(NpgsqlConnection connection, CancellationToken ct = default)
    {
        IPgCommandSource prepared = PrepareCommand(connection);
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(prepared)
        ).AsyncReader(ct);
        await reader.ReadAsync(ct);
        long amount = reader.GetInt64(0);
        return amount;
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

    public void AcceptAscending(string orderingField) { }

    public void AcceptDescending(string orderingField) { }

    public void AcceptTextSearch(string textSearch)
    {
        if (!string.IsNullOrWhiteSpace(textSearch))
        {
            ReadOnlyMemory<float> embeddings = generator.Generate(textSearch);
            _vector = new Vector(embeddings);
            _ordering.WithVectorSearch();
        }
    }

    public IPgCommandSource PrepareCommand(NpgsqlConnection connection)
    {
        NpgsqlCommand command = connection.CreateCommand();
        string sql = GenerateSql();
        command.CommandText = sql;
        foreach (NpgsqlParameter parameter in _parameters)
            command.Parameters.Add(parameter);
        if (_vector != null)
            command.Parameters.AddWithValue("@embedding", _vector);
        return new DefaultPgCommandSource(command);
    }

    public void AcceptPagination(int page) { }

    private string GenerateSql()
    {
        StringBuilder sb = new StringBuilder(_sql);
        sb = sb.AppendLine(GenerateFilters());
        sb = _ordering.Apply(sb);
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
