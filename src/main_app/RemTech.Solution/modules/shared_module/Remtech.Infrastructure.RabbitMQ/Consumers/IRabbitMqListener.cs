using RabbitMQ.Client.Events;

namespace Remtech.Infrastructure.RabbitMQ.Consumers;

public interface IRabbitMqListener
{
    void Configure();
    Task HandleMessage(object sender, BasicDeliverEventArgs eventArgs);
}
