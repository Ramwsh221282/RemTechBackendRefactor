using ContainedItems.Domain.Models;

namespace ContainedItems.Domain.Contracts;

/// <summary>
/// Репозиторий для управления содержащимися элементами.
/// </summary>
public interface IContainedItemsRepository
{
	/// <summary>
	/// Добавляет множество содержащихся элементов.
	/// </summary>
	/// <param name="items">Коллекция содержащихся элементов для добавления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Количество добавленных элементов.</returns>
	Task<int> AddMany(IEnumerable<ContainedItem> items, CancellationToken ct = default);

	/// <summary>
	/// Обновляет множество содержащихся элементов.
	/// </summary>
	/// <param name="items">Коллекция содержащихся элементов для обновления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию обновления.</returns>
	Task UpdateMany(IEnumerable<ContainedItem> items, CancellationToken ct = default);

	/// <summary>
	/// Выполняет запрос для получения содержащихся элементов на основе заданного запроса.
	/// </summary>
	/// <param name="query">Запрос для получения содержащихся элементов.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Массив содержащихся элементов, соответствующих запросу.</returns>
	Task<ContainedItem[]> Query(ContainedItemsQuery query, CancellationToken ct = default);
}
