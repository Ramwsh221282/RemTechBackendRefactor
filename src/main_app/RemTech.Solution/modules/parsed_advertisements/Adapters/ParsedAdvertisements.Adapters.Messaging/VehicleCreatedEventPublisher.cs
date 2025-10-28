using System.Text;
using System.Text.Json;
using ParsedAdvertisements.Domain.VehicleContext.Events;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Messaging;
using RabbitMQ.Client;
using Shared.Infrastructure.Module.Consumers;

namespace ParsedAdvertisements.Adapters.Messaging;

public sealed class VehicleCreatedEventPublisher(RabbitMqConnectionProvider provider) : IVehicleCreatedEventPublisher
{
    public async Task Publish(VehicleCreatedEvent @event)
    {
        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);

        await using var channel = await provider.ProvideChannel();
        await channel.BasicPublishAsync("on-vehicle-created", nameof(VehicleCreatedEvent), true, body);
    }
}