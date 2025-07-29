using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Delegates;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;

public sealed record VehicleCharacteristicsDictionary(
    IEnumerable<VehicleCharacteristicsDictionaryEntry> Characteristics
)
{
    public static async Task<VehicleCharacteristicsDictionary> Read(
        NpgsqlConnection connection,
        VehicleCharacteristicsDictionarySqlQuery query,
        ReadVehicleCharacteristicsDictionary dictionary,
        CancellationToken ct = default
    )
    {
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(query.PrepareCommand(connection))
        ).AsyncReader(ct);
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
