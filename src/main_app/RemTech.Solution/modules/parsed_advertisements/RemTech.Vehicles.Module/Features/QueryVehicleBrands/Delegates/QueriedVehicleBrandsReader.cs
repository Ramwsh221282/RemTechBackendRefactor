using Npgsql;
using RemTech.Vehicles.Module.Features.QueryVehicleBrands.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehicleBrands.Delegates;

public delegate Task<VehicleBrandPresentationReader> QueriedVehicleBrandsReader(
    NpgsqlConnection connection,
    Guid id,
    QueryVehicleBrandsCommand commandSource,
    CancellationToken ct = default
);
