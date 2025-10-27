using Cleaner.WebApi.Events;

namespace Cleaner.WebApi.Messaging;

public interface ICleanerStateUpdatePublisher
{
    Task Publish(CleanerStateUpdatedEvent @event);
}
