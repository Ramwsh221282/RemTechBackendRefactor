using RabbitMQ.Client;

namespace Remtech.Infrastructure.RabbitMQ.Consumers;

public sealed class QueueExchangeSetter
{
    private string _queueName = string.Empty;
    private string _exchangeName = string.Empty;

    public void AddQueueName(string name) => _queueName = name;

    public void AddExchangeName(string exchangeName) => _exchangeName = exchangeName;

    public async Task BindExchangeWithQueue(IChannel channel, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_queueName))
            throw new InvalidOperationException(
                "Queue name not set for exchange binding with queue."
            );
        if (string.IsNullOrWhiteSpace(_exchangeName))
            throw new InvalidOperationException(
                "Exchange name not set for exchange binding with queue."
            );

        await channel.QueueBindAsync(_queueName, _exchangeName, _queueName, cancellationToken: ct);
    }
}
