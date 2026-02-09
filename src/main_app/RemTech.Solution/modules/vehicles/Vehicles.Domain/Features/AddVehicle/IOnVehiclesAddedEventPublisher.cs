namespace Vehicles.Domain.Features.AddVehicle;

/// <summary>
/// Публикатор событий о добавлении транспортных средств.
/// </summary>
public interface IOnVehiclesAddedEventPublisher
{
	/// <summary>
	/// Публикует событие о добавлении транспортных средств.
	/// </summary>
	/// <param name="creatorId">Идентификатор создателя события.</param>
	/// <param name="addedAmount">Количество добавленных транспортных средств.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию публикации события.</returns>
	Task Publish(Guid creatorId, int addedAmount, CancellationToken ct = default);
}
