using RabbitMQ.Client.Events;

namespace Parsing.RabbitMq.StartParsing;

public interface IStartParsingListener
{
    Task Acknowledge(BasicDeliverEventArgs args, CancellationToken ct = default);
    Task Prepare(CancellationToken ct = default);
    Task StartConsuming(CancellationToken ct = default);
    AsyncEventingBasicConsumer Consumer { get; }
}
