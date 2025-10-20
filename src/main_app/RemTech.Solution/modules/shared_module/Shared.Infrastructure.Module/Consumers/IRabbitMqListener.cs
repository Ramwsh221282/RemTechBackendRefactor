using RabbitMQ.Client.Events;

namespace Shared.Infrastructure.Module.Consumers;

public interface IRabbitMqListener
{
    void Configure();
    Task HandleMessage(object sender, BasicDeliverEventArgs eventArgs);
}
