namespace RemTech.ContainedItems.Module.Features.MessageBus;

public interface IAddContainedItemsPublisher
{
    Task Publish(AddContainedItemMessage message);
}
