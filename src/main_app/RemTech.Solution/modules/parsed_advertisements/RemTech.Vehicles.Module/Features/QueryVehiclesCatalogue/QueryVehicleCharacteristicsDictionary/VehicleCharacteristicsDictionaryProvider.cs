using System.Data.Common;
using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary;

public sealed class VehicleCharacteristicsDictionaryProvider(NpgsqlConnection connection)
{
    public async Task<VehicleCharacteristicsDictionary> Provide(
        VehiclesQueryRequest request,
        CancellationToken ct = default
    )
    {
        VehicleCharacteristicsDictionarySqlQuery query = new();
        AsyncDbReaderCommand command = query.CreateCommand(
            request.KindId.Id,
            request.BrandId.Id,
            request.ModelId.Id,
            connection
        );
        await using DbDataReader reader = await command.AsyncReader(ct);
        return await VehicleCharacteristicsDictionary.Read(
            reader,
            VehicleCharacteristicsDictionary.Read(VehicleCharacteristicsDictionaryEntry.Read()),
            ct
        );
    }
}
