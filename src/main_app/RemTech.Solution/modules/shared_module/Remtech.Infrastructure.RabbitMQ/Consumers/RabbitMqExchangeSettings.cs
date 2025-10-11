using RabbitMQ.Client;

namespace Remtech.Infrastructure.RabbitMQ.Consumers;

public sealed class RabbitMqExchangeSettings
{
    private string _name = string.Empty;
    private string _type = string.Empty;
    private bool _isDurable;
    private bool _isAutoDeletable;
    private bool _isPassive;
    private bool _noWait;
    private readonly Dictionary<string, object?> _arguments = [];

    public RabbitMqExchangeSettings WithName(string name)
    {
        _name = name;
        return this;
    }

    public RabbitMqExchangeSettings WithType(string type)
    {
        _type = type;
        return this;
    }

    public RabbitMqExchangeSettings WithDurability()
    {
        _isDurable = true;
        return this;
    }

    public RabbitMqExchangeSettings WithAutoDeletion()
    {
        _isAutoDeletable = true;
        return this;
    }

    public RabbitMqExchangeSettings WithPassive()
    {
        _isPassive = true;
        return this;
    }

    public RabbitMqExchangeSettings WithNoWait()
    {
        _noWait = true;
        return this;
    }

    public RabbitMqExchangeSettings WithArgument(string name, object value)
    {
        _arguments.Add(name, value);
        return this;
    }

    public async Task Declare(IChannel channel, CancellationToken ct = default)
    {
        await channel.ExchangeDeclareAsync(
            exchange: _name,
            type: _type,
            autoDelete: _isAutoDeletable,
            arguments: _arguments,
            noWait: _noWait,
            durable: _isDurable,
            passive: _isPassive,
            cancellationToken: ct
        );
    }

    public async Task BindQueue(
        IChannel channel,
        RabbitMqQueueSettings queueSettings,
        CancellationToken ct = default
    )
    {
        QueueExchangeSetter setter = new QueueExchangeSetter();
        setter.AddExchangeName(_name);
        queueSettings.AddQueueName(setter);
        await setter.BindExchangeWithQueue(channel, ct);
    }
}
