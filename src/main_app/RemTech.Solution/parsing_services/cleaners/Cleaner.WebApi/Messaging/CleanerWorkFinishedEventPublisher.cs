using System.Text;
using System.Text.Json;
using Cleaner.WebApi.Events;
using RabbitMQ.Client;
using Shared.Infrastructure.Module.Consumers;

namespace Cleaner.WebApi.Messaging;

public sealed class CleanerWorkFinishedEventPublisher(RabbitMqConnectionProvider provider)
    : ICleanerWorkFinishedEventPublisher
{
    public async Task Publish(CleanerWorkFinishedEvent @event)
    {
        await using var channel = await provider.ProvideChannel();
        var content = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(content);
        await channel.BasicPublishAsync(
            exchange: "Cleaners",
            routingKey: nameof(CleanerWorkFinishedEvent),
            body: body,
            mandatory: true
        );
    }
}