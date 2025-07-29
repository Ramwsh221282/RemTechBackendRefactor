using Npgsql;
using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary;

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
