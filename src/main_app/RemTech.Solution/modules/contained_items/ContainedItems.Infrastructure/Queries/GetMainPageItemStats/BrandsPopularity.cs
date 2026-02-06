namespace ContainedItems.Infrastructure.Queries.GetMainPageItemStats;

/// <summary>
/// Популярность брендов.
/// </summary>
/// <param name="Id">Идентификатор бренда.</param>
/// <param name="Name">Название бренда.</param>
/// <param name="Count">Количество объявлений техники под брендом.</param>
public sealed record BrandsPopularity(Guid Id, string Name, int Count);
