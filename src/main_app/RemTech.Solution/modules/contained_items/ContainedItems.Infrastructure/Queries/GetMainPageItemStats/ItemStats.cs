namespace ContainedItems.Infrastructure.Queries.GetMainPageItemStats;

/// <summary>
/// Статистика по типам содержащихся элементов.
/// </summary>
/// <param name="ItemType">Тип содержащегося элемента.</param>
/// <param name="Count">Количество элементов данного типа.</param>
public sealed record ItemStats(string ItemType, int Count);
