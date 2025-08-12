namespace RemTech.ContainedItems.Module.Features.QueryRecentContainedItems;

internal interface ISomeItemsSource
{
    Task<IEnumerable<SomeRecentItem>> QueryItems(IEnumerable<QueriedRecentItem> items);
}
