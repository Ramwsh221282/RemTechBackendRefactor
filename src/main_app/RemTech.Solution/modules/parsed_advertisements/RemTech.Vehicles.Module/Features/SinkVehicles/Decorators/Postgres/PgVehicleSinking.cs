﻿using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;
using RemTech.Vehicles.Module.Types.Brands.Decorators.Logic;
using RemTech.Vehicles.Module.Types.GeoLocations.Decorators.Logic;
using RemTech.Vehicles.Module.Types.Kinds.Decorators.Logic;
using RemTech.Vehicles.Module.Types.Models.Decorators.Logic;
using RemTech.Vehicles.Module.Types.Transport;
using RemTech.Vehicles.Module.Types.Transport.Decorators;
using RemTech.Vehicles.Module.Types.Transport.Decorators.Postgres;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;

public sealed class PgVehicleSinking(
    PgConnectionSource connection,
    ITransportAdvertisementSinking sinking
) : ITransportAdvertisementSinking
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
