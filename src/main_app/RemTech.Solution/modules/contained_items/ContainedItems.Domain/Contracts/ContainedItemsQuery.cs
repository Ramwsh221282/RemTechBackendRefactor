namespace ContainedItems.Domain.Contracts;

/// <summary>
/// Запрос для получения содержащихся элементов.
/// </summary>
/// <param name="Status">Статус содержащихся элементов.</param>
/// <param name="Limit">Максимальное количество элементов для получения.</param>
/// <param name="WithLock">Флаг, указывающий, следует ли блокировать элементы при получении.</param>
/// <param name="ItemType">Тип содержащихся элементов.</param>
public sealed record ContainedItemsQuery(
	string? Status = null,
	int? Limit = null,
	bool WithLock = false,
	string? ItemType = null
);
