namespace RemTech.ContainedItems.Module.Features.DropVehicles;

internal sealed class DropContainedItemsDeniedException : Exception
{
    public DropContainedItemsDeniedException()
        : base("Доступ к команде запрещен.") { }
}
