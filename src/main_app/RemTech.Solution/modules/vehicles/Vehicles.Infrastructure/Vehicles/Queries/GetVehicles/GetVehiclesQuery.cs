using RemTech.SharedKernel.Core.Handlers;

namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

public sealed class GetVehiclesQuery(GetVehiclesQueryParameters parameters) : IQuery
{
	public GetVehiclesQueryParameters Parameters { get; } = parameters;

	public override string ToString() => Parameters.ToString();
}
