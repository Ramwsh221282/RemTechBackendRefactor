using RemTech.Core.Shared.Result;
using RemTech.Vehicles.Module.Types.Characteristics;
using RemTech.Vehicles.Module.Types.Characteristics.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

internal sealed class PgCharacteristicsSinking(ITransportAdvertisementSinking sinking)
    : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        Characteristic[] ctxes = Converted(sink);
        VehicleIdentity identity = sink.VehicleId();
        IItemPrice price = sink.VehiclePrice();
        VehiclePhotos photos = sink.VehiclePhotos();
        string sourceUrl = sink.SourceUrl();
        string sourceDomain = sink.SourceDomain();
        string sentences = sink.Sentences();
        Vehicle vehicle = new(identity, price, photos, sourceUrl, sourceDomain, sentences);
        vehicle = ctxes.Aggregate(vehicle, (current, entry) => entry.ToVehicle(current));
        return await sinking.Sink(new CachedVehicleJsonSink(sink, vehicle), ct);
    }

    private Characteristic[] Converted(IVehicleJsonSink sink)
    {
        return sink.Characteristics()
            .Select(c =>
            {
                CharacteristicText name = new CharacteristicText(c.Name);
                CharacteristicIdentity identity = new CharacteristicIdentity(name);
                VehicleCharacteristicValue value = new VehicleCharacteristicValue(c.Value);
                Characteristic characteristic = new Characteristic(identity, value);
                return characteristic;
            })
            .ToArray();
    }
}
