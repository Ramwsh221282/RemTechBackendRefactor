using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary;

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
