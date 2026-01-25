namespace ContainedItems.Infrastructure.Queries.GetMainPageItemStats;

public sealed record MainPageItemStatsResponse(
	IReadOnlyList<ItemStats> ItemStats,
	IReadOnlyList<BrandsPopularity> BrandsPopularity,
	IReadOnlyList<CategoriesPopularity> CategoriesPopularity
);
