using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators.Postgres;
using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement.Decorators.Postgres;

public sealed class PgVehicleSinking(
    PgConnectionSource connection, 
    ITransportAdvertisementSinking sinking)
    : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        VehicleBrand brand = sink.Brand();
        VehicleKind kind = sink.Kind();
        VehicleModel model = sink.Model();
        GeoLocation location = sink.Location();
        Vehicle vehicle = location.Print(
            model.Print(
                kind.Print(
                    brand.Print(
                        sink.Vehicle()))));
        await new PgTransactionalVehicle(connection, new ValidVehicle(vehicle))
            .SaveAsync(ct);
        return await sinking.Sink(sink, ct);
    }
}