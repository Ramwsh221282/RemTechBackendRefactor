using Npgsql;
using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;
using RemTech.Vehicles.Module.Types.Characteristics;
using RemTech.Vehicles.Module.Types.Characteristics.Adapters.Storage.Postgres;
using RemTech.Vehicles.Module.Types.Characteristics.Features.Structuring;
using RemTech.Vehicles.Module.Types.Transport;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

public sealed class PgCharacteristicsSinking(
    NpgsqlDataSource connection,
    ITransportAdvertisementSinking sinking
) : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        Characteristic[] ctxes = Unveiled(sink);
        Characteristic[] stored = await Stored(ctxes, ct);
        VehicleIdentity identity = sink.VehicleId();
        IItemPrice price = sink.VehiclePrice();
        VehiclePhotos photos = sink.VehiclePhotos();
        Vehicle vehicle = new(identity, price, photos);
        foreach (Characteristic entry in stored)
        {
            try
            {
                vehicle = entry.ToVehicle(vehicle);
            }
            catch
            {
                // ignored
            }
        }

        return await sinking.Sink(new CachedVehicleJsonSink(sink, vehicle), ct);
    }

    private Characteristic[] Unveiled(IVehicleJsonSink sink)
    {
        CharacteristicVeil[] ctxes = sink.Characteristics();
        UniqueCharacteristics unique = new();
        foreach (var ctx in ctxes)
        {
            try
            {
                unique = ctx.Characteristic().Print(unique);
            }
            catch
            {
                // ignored
            }
        }

        return unique.ReadAll();
    }

    private async Task<Characteristic[]> Stored(Characteristic[] ctxes, CancellationToken ct)
    {
        List<Characteristic> results = [];
        foreach (Characteristic entry in ctxes)
        {
            try
            {
                Characteristic stored = await new PgVarietCharacteristicsStorage()
                    .With(new PgCharacteristicsStorage(connection))
                    .With(new PgDuplicateResolvingCharacteristicsStorage(connection))
                    .Stored(entry, ct);
                results.Add(stored);
            }
            catch
            {
                // ignored
            }
        }
        return results.ToArray();
    }
}
