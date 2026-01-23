using System.Diagnostics;
using ContainedItems.Infrastructure.Producers;

namespace ContainedItems.Infrastructure.BackgroundServices;

public sealed class ItemPublishStrategyFactory(AddSparesProducer addSpares, AddVehiclesProducer addVehicles)
{
	private AddSparesProducer Spares { get; } = addSpares;
	private AddVehiclesProducer Vehicles { get; } = addVehicles;

	public IItemPublishingStrategy Resolve(string itemType)
	{
		return itemType switch
		{
			"Запчасти" => Spares,
			"Техника" => Vehicles,
			_ => throw new UnreachableException("Unknown item type: " + itemType),
		};
	}
}
