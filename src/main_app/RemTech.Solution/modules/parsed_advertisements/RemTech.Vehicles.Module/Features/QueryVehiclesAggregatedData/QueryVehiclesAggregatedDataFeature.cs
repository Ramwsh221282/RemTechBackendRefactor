using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Vehicles.Module.Database.SqlStringGeneration;
using RemTech.Vehicles.Module.Features.QueryVehicles;
using RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;
using RemTech.Vehicles.Module.Features.QueryVehicles.Extensions;
using RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;
using Shared.Infrastructure.Module.Postgres.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesAggregatedData;

public static class QueryVehiclesAggregatedDataFeature
{
    private const int MaxPageSize = 20;

    public sealed record VehiclesAggregatedDataQueryRequest(
        VehicleKindIdQueryFilterArgument KindId,
        VehicleBrandIdQueryFilterArgument BrandId,
        VehicleModelIdQueryFilterArgument? ModelId = null,
        VehicleRegionIdQueryFilterArgument? RegionId = null,
        VehiclePriceQueryFilterArgument? Price = null,
        VehicleCharacteristicsQueryArguments? Characteristics = null
    ) : VehicleQueryFilterArgument
    {
        public override CompositeVehicleSpeicification ApplyTo(
            CompositeVehicleSpeicification speicification
        )
        {
            CompositeVehicleSpeicification composite = new();
            composite = KindId.ApplyTo(composite);
            composite = BrandId.ApplyTo(composite);
            composite = ModelId.ApplyIfProvided(composite);
            composite = RegionId.ApplyIfProvided(composite);
            composite = Price.ApplyIfProvided(composite);
            composite = RegionId.ApplyIfProvided(composite);
            composite = Price.ApplyIfProvided(composite);
            composite = Characteristics.ApplyIfProvided(composite);
            return composite;
        }
    }

    public sealed record VehiclesAggregatedDataResult(
        int TotalCount,
        double AveragePrice,
        double MinimalPrice,
        double MaximalPrice,
        int PagesCount
    );

    public static void Map(RouteGroupBuilder builder) =>
        builder.MapPost("kinds/{kindId:Guid}/brands/{brandId:Guid}/catalogue/aggregated", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource connectionSource,
        [FromRoute] Guid kindId,
        [FromRoute] Guid brandId,
        [FromBody] VehiclesAggregatedDataQueryRequest request,
        CancellationToken ct
    )
    {
        if (kindId == Guid.Empty || brandId == Guid.Empty)
            return Results.NoContent();
        request = request with
        {
            KindId = new VehicleKindIdQueryFilterArgument(kindId),
            BrandId = new VehicleBrandIdQueryFilterArgument(brandId),
        };
        VehiclesAggregatedDataResult result = await ExecuteQuery(connectionSource, request, ct);
        return Results.Ok(result);
    }

    private static async Task<VehiclesAggregatedDataResult> ExecuteQuery(
        NpgsqlDataSource connectionSource,
        VehiclesAggregatedDataQueryRequest request,
        CancellationToken ct
    )
    {
        await using NpgsqlConnection connection = await connectionSource.OpenConnectionAsync(ct);
        AsyncDbReaderCommand command = CreateCommand(connection, request);
        VehiclesAggregatedDataResult result = await ExecuteCommand(command, ct);
        return result;
    }

    private sealed class VehiclesAggregatedDataSqlQuery : IVehiclesSqlQuery
    {
        private readonly List<string> _filters = [];
        private readonly List<NpgsqlParameter> _parameters = [];
        private readonly string _sql = string.Intern(
            """
            SELECT
            COUNT(v.id) as total_count,
            AVG(v.price) as average_price,
            MIN(v.price) as min_price,
            MAX(v.price) as max_price    
            FROM parsed_advertisements_module.parsed_vehicles v                                    
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

        public void AcceptAscending(string orderingField) { }

        public void AcceptDescending(string orderingField) { }

        public IPgCommandSource PrepareCommand(NpgsqlConnection connection)
        {
            string sql = SqlGenerator.SourceSql(_sql).ApplyFilters(new SqlFilters(_filters))().Sql;
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = sql;
            foreach (NpgsqlParameter parameter in _parameters)
                command.Parameters.Add(parameter);
            return new DefaultPgCommandSource(command);
        }

        public void AcceptPagination(int page) { }

        public void AcceptTextSearch(string textSearch) { }

        public VehiclesAggregatedDataSqlQuery AcceptRequest(
            VehiclesAggregatedDataQueryRequest request
        )
        {
            CompositeVehicleSpeicification speicification = new();
            speicification = request.ApplyTo(speicification);
            speicification.ApplyTo(this);
            return this;
        }
    }

    private static AsyncDbReaderCommand CreateCommand(
        NpgsqlConnection connection,
        VehiclesAggregatedDataQueryRequest request
    )
    {
        VehiclesAggregatedDataSqlQuery query = new();
        query = query.AcceptRequest(request);
        return new AsyncDbReaderCommand(new AsyncPreparedCommand(query.PrepareCommand(connection)));
    }

    private static async Task<VehiclesAggregatedDataResult> ExecuteCommand(
        AsyncDbReaderCommand command,
        CancellationToken ct = default
    )
    {
        await using DbDataReader reader = await command.AsyncReader(ct);
        if (!await reader.ReadAsync(ct))
            return new VehiclesAggregatedDataResult(0, 0, 0, 0, 0);
        return new VehiclesAggregatedDataResult(
            reader.GetInt32(reader.GetOrdinal("total_count")),
            reader.GetDouble(reader.GetOrdinal("average_price")),
            reader.GetDouble(reader.GetOrdinal("min_price")),
            reader.GetDouble(reader.GetOrdinal("max_price")),
            (int)
                Math.Ceiling(
                    reader.GetInt32(reader.GetOrdinal("total_count")) / (double)MaxPageSize
                )
        );
    }
}
