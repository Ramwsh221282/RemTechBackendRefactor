using ContainedItems.Domain.Models;

namespace ContainedItems.Infrastructure.BackgroundServices;

/// <summary>
/// Стратегия публикации содержащихся элементов.
/// </summary>
public interface IItemPublishingStrategy
{
	/// <summary>
	/// Публикует содержащийся элемент.
	/// </summary>
	/// <param name="item">Содержащийся элемент для публикации.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию публикации.</returns>
	Task Publish(ContainedItem item, CancellationToken ct = default);

	/// <summary>
	/// Публикует множество содержащихся элементов.
	/// </summary>
	/// <param name="items">Содержащиеся элементы для публикации.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию публикации.</returns>
	Task PublishMany(IEnumerable<ContainedItem> items, CancellationToken ct = default);
}
