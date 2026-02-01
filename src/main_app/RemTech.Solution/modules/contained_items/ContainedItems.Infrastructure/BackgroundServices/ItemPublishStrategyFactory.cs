using System.Diagnostics;
using ContainedItems.Infrastructure.Producers;

namespace ContainedItems.Infrastructure.BackgroundServices;

/// <summary>
/// Фабрика стратегий публикации содержащихся элементов.
/// </summary>
/// <param name="addSpares">Производитель для запчастей.</param>
/// <param name="addVehicles">Производитель для техники.</param>
public sealed class ItemPublishStrategyFactory(AddSparesProducer addSpares, AddVehiclesProducer addVehicles)
{
	private AddSparesProducer Spares { get; } = addSpares;
	private AddVehiclesProducer Vehicles { get; } = addVehicles;

	/// <summary>
	/// Разрешает стратегию публикации по типу элемента.
	/// </summary>
	/// <param name="itemType">Тип содержащегося элемента.</param>
	/// <returns>Стратегия публикации для указанного типа элемента.</returns>
	/// <exception cref="UnreachableException">Выбрасывается, если тип элемента неизвестен.</exception>
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
