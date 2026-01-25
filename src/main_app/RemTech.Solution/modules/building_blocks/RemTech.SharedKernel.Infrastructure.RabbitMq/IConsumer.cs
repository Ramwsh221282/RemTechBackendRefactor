using RabbitMQ.Client;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public interface IConsumer
{
	public Task InitializeChannel(IConnection connection, CancellationToken ct = default);
	public Task StartConsuming(CancellationToken ct = default);
	public Task Shutdown(CancellationToken ct = default);
}
