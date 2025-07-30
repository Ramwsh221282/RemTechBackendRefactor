using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations.Delegates;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations;

public static class CatalogueGeoLocationsSource
{
    public static async Task<IEnumerable<CatalogueGeoLocationPresentation>> Provide(
        this NpgsqlConnection connection,
        VehiclesQueryRequest request,
        CatalogueGeoLocationCommandSource commandSource,
        CatalogueGeoLocationsReaderSource readerSource,
        CatalogueGeoLocationsReadingSource readingSource,
        CancellationToken ct = default
    )
    {
        AsyncDbReaderCommand command = commandSource(connection, request);
        await using CatalogueGeoLocationsReader reader = await readerSource(command, ct);
        return await readingSource(reader, ct);
    }

    public static CatalogueGeoLocationCommandSource CatalogueGeoLocationCommandSource =>
        (connection, request) =>
        {
            CatalogueLocationsSqlQuery query = new();
            query = query.ApplyRequest(request);
            IPgCommandSource command = query.PrepareCommand(connection);
            return new AsyncDbReaderCommand(new AsyncPreparedCommand(command));
        };

    public static CatalogueGeoLocationsReaderSource CatalogueGeoLocationsReaderSource =>
        async (command, ct) => new CatalogueGeoLocationsReader(await command.AsyncReader(ct));

    public static CatalogueGeoLocationsReadingSource CatalogueGeoLocationsReadingSource =>
        (reader, ct) => reader.Read(ct);
}
