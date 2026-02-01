using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace Spares.Domain.Features;

/// <summary>
/// Транспортировщик событий добавления запчастей.
/// </summary>
/// <param name="publisher">Публикатор событий добавления запчастей.</param>
public sealed class AddSparesEventTransporter(IOnSparesAddedEventPublisher publisher)
	: IEventTransporter<AddSparesCommand, (Guid, int)>
{
	/// <summary>
	///     Транспортирует событие добавления запчастей.
	/// </summary>
	/// <param name="result">Результат добавления запчастей, содержащий идентификатор и количество добавленных элементов.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию транспортировки события.</returns>
	public async Task Transport((Guid, int) result, CancellationToken ct = default)
	{
		if (result.Item1 == Guid.Empty)
		{
			return;
		}
		if (result.Item2 == 0)
		{
			return;
		}

		await publisher.Publish(result.Item1, result.Item2, ct);
	}
}
