namespace ContainedItems.Infrastructure.Queries.GetMainPageLastAddedItems;

/// <summary>
/// Ответ с последними добавленными элементами на главную страницу.
/// </summary>
/// <param name="Items">Коллекция последних добавленных элементов.</param>
public sealed record MainPageLastAddedItemsResponse(IEnumerable<MainPageLastAddedItem> Items);
