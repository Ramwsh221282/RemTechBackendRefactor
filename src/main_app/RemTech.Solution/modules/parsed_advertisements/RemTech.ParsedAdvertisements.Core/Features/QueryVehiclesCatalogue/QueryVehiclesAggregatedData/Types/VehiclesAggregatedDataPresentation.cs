using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Delegates;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Types;

public sealed record VehiclesAggregatedDataPresentation(
    int TotalCount,
    double AveragePrice,
    double MinimalPrice,
    double MaximalPrice,
    int PagesCount
)
{
    private const int MaxPageSize = 20;

    public static async Task<VehiclesAggregatedDataPresentation> Read(
        VehiclesAggregatedDataSqlQuery query,
        NpgsqlConnection connection,
        CancellationToken ct = default
    )
    {
        IPgCommandSource command = query.PrepareCommand(connection);
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(command)
        ).AsyncReader(ct);
        VehiclesAggregatedDataReading reading = Read(reader);
        VehiclesAggregatedDataPresentation data = await reading(reader, ct);
        return data;
    }

    private static VehiclesAggregatedDataReading Read(DbDataReader reader)
    {
        return async (dataReader, ct) =>
        {
            if (!await reader.ReadAsync(ct))
                return await ReadNothing()(dataReader, ct);
            return new VehiclesAggregatedDataPresentation(
                reader.GetInt32(reader.GetOrdinal("total_count")),
                reader.GetDouble(reader.GetOrdinal("average_price")),
                reader.GetDouble(reader.GetOrdinal("min_price")),
                reader.GetDouble(reader.GetOrdinal("max_price")),
                (int)
                    Math.Ceiling(
                        reader.GetInt32(reader.GetOrdinal("total_count")) / (double)MaxPageSize
                    )
            );
        };
    }

    private static VehiclesAggregatedDataReading ReadNothing()
    {
        return (_, _) =>
        {
            VehiclesAggregatedDataPresentation empty = new VehiclesAggregatedDataPresentation(
                0,
                0,
                0,
                0,
                0
            );
            return Task.FromResult(empty);
        };
    }
}
