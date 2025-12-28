namespace ContainedItems.Domain.Features.AddContainedItems;

public sealed record AddContainedItemsBody(
    string ServiceItemId, 
    Guid CreatorId, 
    string CreatorDomain, 
    string CreatorType,
    string Content);