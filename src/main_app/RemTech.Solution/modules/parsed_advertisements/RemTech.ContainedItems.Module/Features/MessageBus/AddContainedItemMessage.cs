namespace RemTech.ContainedItems.Module.Features.MessageBus;

public sealed record AddContainedItemMessage(
    string Id,
    string Type,
    string Domain,
    string SourceUrl
);
