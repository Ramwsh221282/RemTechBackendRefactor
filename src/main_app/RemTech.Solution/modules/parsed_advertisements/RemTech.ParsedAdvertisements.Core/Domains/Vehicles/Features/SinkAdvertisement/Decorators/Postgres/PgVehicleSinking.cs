using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Decorators.Logic;
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
        Vehicle vehicle = sink.Vehicle();
        Vehicle locationed = new LocationingGeoLocation(sink.Location()).Locatate(vehicle);
        Vehicle kinded = new KindingVehicleKind(sink.Kind()).KindVehicle(locationed);
        Vehicle branded = new BrandingVehicleBrand(sink.Brand()).BrandVehicle(kinded);
        Vehicle modeled = new ModelingVehicleModel(sink.Model()).ModeledVehicle(branded);
        Vehicle valid = new ValidVehicle(modeled);
        await new PgTransactionalVehicle(connection, valid).SaveAsync(ct);
        return await sinking.Sink(sink, ct);
    }
}