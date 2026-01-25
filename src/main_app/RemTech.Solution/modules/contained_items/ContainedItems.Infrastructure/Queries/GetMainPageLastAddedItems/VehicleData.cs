namespace ContainedItems.Infrastructure.Queries.GetMainPageLastAddedItems;

public sealed record VehicleData(
	Guid Id,
	string Title,
	IEnumerable<string> Photos,
	IEnumerable<VehicleDataCharacteristic> Characteristics
);
