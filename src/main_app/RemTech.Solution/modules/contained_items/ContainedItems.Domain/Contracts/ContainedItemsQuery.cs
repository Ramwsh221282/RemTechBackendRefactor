namespace ContainedItems.Domain.Contracts;

public sealed record ContainedItemsQuery(
    string? Status = null, 
    int? Limit = null, 
    bool WithLock = false,
    string? ItemType = null);