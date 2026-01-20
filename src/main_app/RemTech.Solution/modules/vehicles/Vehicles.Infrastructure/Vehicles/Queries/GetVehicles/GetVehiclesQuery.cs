using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

public sealed record GetVehiclesQuery(GetVehiclesQueryParameters Parameters) : IQuery;
