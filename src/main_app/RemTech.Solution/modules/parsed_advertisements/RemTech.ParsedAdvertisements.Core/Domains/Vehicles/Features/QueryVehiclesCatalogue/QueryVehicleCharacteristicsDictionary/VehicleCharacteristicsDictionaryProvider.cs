using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary;

public sealed class VehicleCharacteristicsDictionaryProvider(NpgsqlConnection connection)
{
    public async Task<VehicleCharacteristicsDictionary> Provide(
        VehiclesQueryRequest request,
        CancellationToken ct = default
    ) =>
        await VehicleCharacteristicsDictionary.Read(
            connection,
            new VehicleCharacteristicsDictionarySqlQuery().ApplyRequest(request),
            VehicleCharacteristicsDictionary.Read(VehicleCharacteristicsDictionaryEntry.Read()),
            ct
        );
}
