using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.Delegates;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue;

public static class VehiclesCatalogueProvider
{
    public static async Task<VehiclesCataloguePresentation> QueryCatalogue(
        VehiclesCatalogue catalogue,
        VehiclesOfCatalogue vehicles,
        CharacteristicsOfCatalogue characteristics,
        AggregatedDataOfCatalogue aggregatedData
    ) => await catalogue(vehicles, characteristics, aggregatedData);

    public static VehiclesCatalogue VehiclesCatalogue() =>
        async (vehicles, characteristics, aggregatedData) =>
            new VehiclesCataloguePresentation(
                await vehicles(),
                await characteristics(),
                await aggregatedData()
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
