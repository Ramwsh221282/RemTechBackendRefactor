namespace ContainedItems.Infrastructure.Queries.GetMainPageItemStats;

/// <summary>
/// Ответ с данными статистики основных страниц.
/// </summary>
/// <param name="ItemStats">Статистика по типам содержащихся элементов.</param>
/// <param name="BrandsPopularity">Популярность брендов.</param>
/// <param name="CategoriesPopularity">Популярность категорий.</param>
public sealed record MainPageItemStatsResponse(
	IReadOnlyList<ItemStats> ItemStats,
	IReadOnlyList<BrandsPopularity> BrandsPopularity,
	IReadOnlyList<CategoriesPopularity> CategoriesPopularity
);
