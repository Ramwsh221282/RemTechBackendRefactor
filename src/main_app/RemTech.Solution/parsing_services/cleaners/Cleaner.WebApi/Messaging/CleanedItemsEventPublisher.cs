using System.Text;
using System.Text.Json;
using Cleaner.WebApi.Events;
using RabbitMQ.Client;
using Shared.Infrastructure.Module.Consumers;

namespace Cleaner.WebApi.Messaging;

public sealed class CleanedItemsEventPublisher(RabbitMqConnectionProvider provider)
    : ICleanedItemsEventPublisher
{
    public async Task Publish(CleanerItemsCleanedEvent @event)
    {
        await using IChannel channel = await provider.ProvideChannel();
        var content = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(content);
        await channel.BasicPublishAsync("Cleaners", nameof(CleanerItemsCleanedEvent), body);
    }
}
