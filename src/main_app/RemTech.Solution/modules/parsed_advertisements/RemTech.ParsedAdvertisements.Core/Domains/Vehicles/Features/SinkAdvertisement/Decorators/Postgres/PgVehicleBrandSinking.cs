﻿using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators.Validation;
using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement.Decorators.Postgres;

public sealed class PgVehicleBrandSinking(PgConnectionSource connection, ITransportAdvertisementSinking sinking)
    : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        VehicleBrand brand = sink.Brand();
        VehicleBrand valid = new ValidVehicleBrand(brand);
        VehicleBrand saved = await new PgVarietVehicleBrand(connection, valid).SaveAsync(ct);
        return await sinking.Sink(new CachedVehicleJsonSink(sink, saved), ct);
    }
}