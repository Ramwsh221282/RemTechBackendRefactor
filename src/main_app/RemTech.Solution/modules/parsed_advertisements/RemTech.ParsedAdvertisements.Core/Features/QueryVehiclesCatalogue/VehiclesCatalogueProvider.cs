using Npgsql;
using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.Delegates;
using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary;
using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles;
using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehiclesAggregatedData;
using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.Types;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue;

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
