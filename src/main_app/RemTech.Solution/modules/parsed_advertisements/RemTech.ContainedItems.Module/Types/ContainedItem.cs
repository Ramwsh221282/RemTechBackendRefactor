namespace RemTech.ContainedItems.Module.Types;

internal sealed record ContainedItem(
    string Id,
    string Type,
    string Domain,
    DateTime CreatedAt,
    bool IsDeleted,
    string SourceUrl
) : IContainedItem
{
    public static ContainedItem New(string id, string type, string domain, string sourceUrl)
    {
        return new ContainedItem(id, type, domain, DateTime.UtcNow, false, sourceUrl);
    }
}
