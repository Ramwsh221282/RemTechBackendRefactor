using RabbitMQ.Client;

namespace Remtech.Infrastructure.RabbitMQ.Consumers;

public sealed class RabbitMqQueueSettings
{
    private readonly Dictionary<string, object?> _arguments = [];
    private string _name = string.Empty;
    private bool _isDurable;
    private bool _isExclusive;
    private bool _isAutoDeletable;

    public RabbitMqQueueSettings WithName(string name)
    {
        _name = name;
        return this;
    }

    public RabbitMqQueueSettings WithDurability()
    {
        _isDurable = true;
        return this;
    }

    public RabbitMqQueueSettings ShouldBeExclusive()
    {
        _isExclusive = true;
        return this;
    }

    public RabbitMqQueueSettings ShouldBeAutoDeletable()
    {
        _isAutoDeletable = true;
        return this;
    }

    public RabbitMqQueueSettings WithArgument(string name, object value)
    {
        _arguments.Add(name, value);
        return this;
    }

    public void AddQueueName(QueueExchangeSetter setter)
    {
        setter.AddQueueName(_name);
    }

    public void AddQueueName(RabbitMqConsumer consumer)
    {
        consumer.AddQueue(_name);
    }

    public async Task Declare(IChannel channel, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_name))
            throw new InvalidOperationException("Queue Name cannot be null or whitespace");
        await channel.QueueDeclareAsync(
            queue: _name,
            durable: _isDurable,
            exclusive: _isExclusive,
            autoDelete: _isAutoDeletable,
            arguments: _arguments,
            cancellationToken: ct
        );
    }
}
