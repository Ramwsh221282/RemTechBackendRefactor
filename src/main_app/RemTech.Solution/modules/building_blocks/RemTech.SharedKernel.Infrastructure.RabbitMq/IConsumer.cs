using RabbitMQ.Client;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public interface IConsumer
{
	Task InitializeChannel(IConnection connection, CancellationToken ct = default);
	Task StartConsuming(CancellationToken ct = default);
	Task Shutdown(CancellationToken ct = default);
}
