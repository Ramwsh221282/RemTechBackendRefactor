namespace ContainedItems.Infrastructure.Queries.GetMainPageItemStats;

/// <summary>
/// Популярность категорий.
/// </summary>
/// <param name="Id">Идентификатор категории.</param>
/// <param name="Name">Название категории.</param>
/// <param name="Count">Количество объявлений техники в категории.</param>
public sealed record CategoriesPopularity(Guid Id, string Name, int Count);
