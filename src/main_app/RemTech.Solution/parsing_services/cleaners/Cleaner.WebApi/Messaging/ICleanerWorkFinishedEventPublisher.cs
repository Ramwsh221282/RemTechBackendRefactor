using Cleaner.WebApi.Events;

namespace Cleaner.WebApi.Messaging;

public interface ICleanerWorkFinishedEventPublisher
{
    Task Publish(CleanerWorkFinishedEvent @event);
}
