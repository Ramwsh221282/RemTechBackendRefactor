namespace Vehicles.Domain.Features.AddVehicle;

public sealed record AddVehicleVehiclesCommandPayload(
	Guid Id,
	string Title,
	string Url,
	long Price,
	bool IsNds,
	string Address,
	IReadOnlyList<string> Photos,
	IReadOnlyList<AddVehicleCommandCharacteristics> Characteristics
);
