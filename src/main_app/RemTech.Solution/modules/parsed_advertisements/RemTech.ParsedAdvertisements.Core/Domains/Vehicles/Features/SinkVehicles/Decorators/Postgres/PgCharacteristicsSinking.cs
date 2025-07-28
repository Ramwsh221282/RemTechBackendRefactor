using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkVehicles.Decorators.Postgres;

public sealed class PgCharacteristicsSinking(PgConnectionSource connection, ITransportAdvertisementSinking sinking)
    : ITransportAdvertisementSinking
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