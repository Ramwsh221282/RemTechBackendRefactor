namespace RemTech.ContainedItems.Module.Types;

internal interface IContainedItem
{
    string Id { get; }
    string Type { get; }
    string Domain { get; }
    DateTime CreatedAt { get; }
    bool IsDeleted { get; }
    string SourceUrl { get; }
}
