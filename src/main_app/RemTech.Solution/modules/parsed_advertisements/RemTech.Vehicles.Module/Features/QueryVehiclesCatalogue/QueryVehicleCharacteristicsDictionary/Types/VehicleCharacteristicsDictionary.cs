using System.Data.Common;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Delegates;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;

public sealed record VehicleCharacteristicsDictionary(
    IEnumerable<VehicleCharacteristicsDictionaryEntry> Characteristics
)
{
    public static async Task<VehicleCharacteristicsDictionary> Read(
        DbDataReader reader,
        ReadVehicleCharacteristicsDictionary dictionary,
        CancellationToken ct = default
    )
    {
        return await dictionary(reader, ct);
    }

    public static ReadVehicleCharacteristicsDictionary Read(
        ReadVehicleCharacteristicsDictionaryEntry readVehicleCharacteristicsDictionaryEntry
    ) =>
        async (dataReader, token) =>
        {
            Dictionary<Guid, VehicleCharacteristicsDictionaryEntry> entries = [];
            while (await dataReader.ReadAsync(token))
            {
                VehicleCharacteristicsDictionaryEntry entry =
                    readVehicleCharacteristicsDictionaryEntry(dataReader);
                if (entries.TryAdd(entry.Id, entry))
                    continue;

                entries[entry.Id] = entries[entry.Id] with
                {
                    Values = new VehicleCharacteristicsDictionaryEntryValues(
                        [.. entries[entry.Id].Values.Values, .. entry.Values.Values]
                    ),
                };
            }

            return new VehicleCharacteristicsDictionary(entries.Values.AsEnumerable());
        };
}
