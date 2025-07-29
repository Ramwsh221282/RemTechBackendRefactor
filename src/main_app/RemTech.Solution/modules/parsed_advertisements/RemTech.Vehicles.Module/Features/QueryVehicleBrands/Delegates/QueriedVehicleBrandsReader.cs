using Npgsql;

namespace RemTech.Vehicles.Module.Features.QueryVehicleBrands.Delegates;

public delegate Task<VehicleBrandPresentationReader> QueriedVehicleBrandsReader(
    NpgsqlConnection connection,
    Guid id,
    QueryVehicleBrandsCommand commandSource,
    CancellationToken ct = default
);
