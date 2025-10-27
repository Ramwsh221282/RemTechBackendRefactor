using System.Text;
using System.Text.Json;
using Cleaner.WebApi.Events;
using RabbitMQ.Client;
using Shared.Infrastructure.Module.Consumers;

namespace Cleaner.WebApi.Messaging;

public sealed class CleanerStateUpdateEventPublisher(RabbitMqConnectionProvider provider)
    : ICleanerStateUpdatePublisher
{
    public async Task Publish(CleanerStateUpdatedEvent @event)
    {
        await using IChannel channel = await provider.ProvideChannel();
        var content = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(content);
        await channel.BasicPublishAsync("Cleaners", nameof(CleanerStateUpdatedEvent), body);
    }
}
