using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.Delegates;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations.Types;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData.Types;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue;

public static class VehiclesCatalogueProvider
{
    public static async Task<VehiclesCataloguePresentation> QueryCatalogue(
        VehiclesCatalogue catalogue,
        VehiclesOfCatalogue vehicles,
        CharacteristicsOfCatalogue characteristics,
        AggregatedDataOfCatalogue aggregatedData,
        GeoLocationsOfCatalogue geoLocations
    ) => await catalogue(vehicles, characteristics, aggregatedData, geoLocations);

    public static VehiclesCatalogue VehiclesCatalogue() =>
        async (vehicles, characteristics, aggregatedData, geoLocations) =>
        {
            IEnumerable<VehiclePresentation> vehicleItems = await vehicles();
            VehicleCharacteristicsDictionary characteristicItems = await characteristics();
            VehiclesAggregatedDataPresentation aggregatedDataItems = await aggregatedData();
            IEnumerable<CatalogueGeoLocationPresentation> geoLocationItems = await geoLocations();
            return new VehiclesCataloguePresentation(
                vehicleItems,
                characteristicItems,
                aggregatedDataItems,
                geoLocationItems
            );
        };

    public static GeoLocationsOfCatalogue GeoLocationsOfCatalogue(
        this NpgsqlConnection connection,
        VehiclesQueryRequest request,
        CancellationToken ct = default
    ) =>
        () =>
            connection.Provide(
                request,
                CatalogueGeoLocationsSource.CatalogueGeoLocationCommandSource,
                CatalogueGeoLocationsSource.CatalogueGeoLocationsReaderSource,
                CatalogueGeoLocationsSource.CatalogueGeoLocationsReadingSource,
                ct: ct
            );

    public static VehiclesOfCatalogue VehiclesOfCatalogue(
        this NpgsqlConnection connection,
        Serilog.ILogger logger,
        VehiclesQueryRequest request,
        CancellationToken ct = default
    ) => () => new PgVehiclesProvider(connection, logger).Provide(request, ct);

    public static CharacteristicsOfCatalogue CharacteristicsOfCatalogue(
        this NpgsqlConnection connection,
        VehiclesQueryRequest request,
        CancellationToken ct = default
    ) => () => new VehicleCharacteristicsDictionaryProvider(connection).Provide(request, ct);

    public static AggregatedDataOfCatalogue AggregatedDataOfCatalogue(
        this NpgsqlConnection connection,
        VehiclesQueryRequest request,
        CancellationToken ct = default
    ) => () => new PgVehiclesAggregatedDataProvider(connection).Provide(request, ct);
}
