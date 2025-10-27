using Cleaner.WebApi.Events;

namespace Cleaner.WebApi.Messaging;

public interface ICleanedItemsEventPublisher
{
    Task Publish(CleanerItemsCleanedEvent @event);
}
