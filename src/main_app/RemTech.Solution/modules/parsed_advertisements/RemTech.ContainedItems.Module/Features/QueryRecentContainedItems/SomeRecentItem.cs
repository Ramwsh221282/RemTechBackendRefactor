namespace RemTech.ContainedItems.Module.Features.QueryRecentContainedItems;

internal abstract record SomeRecentItem
{
    public string Id { get; protected init; } = string.Empty;
    public string Region { get; protected init; } = string.Empty;
    public string RegionKind { get; protected init; } = string.Empty;
    public string City { get; protected init; } = string.Empty;
    public long Price { get; protected init; } = -1;
    public bool IsNds { get; protected init; }
    public string SourceUrl { get; protected init; } = string.Empty;
    public IEnumerable<string> Photos { get; protected init; } = [];
    public string Description { get; protected init; } = string.Empty;
}
