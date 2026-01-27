using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace Vehicles.Domain.Features.AddVehicle;

/// <summary>
/// Транспортировщик событий добавления транспортного средства.
/// </summary>
/// <param name="publisher">Публикатор событий добавления транспортного средства.</param>
public sealed class AddVehicleEventTransporter(IOnVehiclesAddedEventPublisher publisher)
	: IEventTransporter<AddVehicleCommand, (Guid, int)>
{
	/// <summary>
	/// Транспортирует событие добавления транспортного средства.
	/// </summary>
	/// <param name="result">Результат добавления транспортного средства, содержащий идентификатор и количество добавленных элементов.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию транспортировки события.</returns>
	public async Task Transport((Guid, int) result, CancellationToken ct = default)
	{
		if (result.Item1 == Guid.Empty)
			return;
		if (result.Item2 == 0)
			return;
		await publisher.Publish(result.Item1, result.Item2, ct);
	}
}
