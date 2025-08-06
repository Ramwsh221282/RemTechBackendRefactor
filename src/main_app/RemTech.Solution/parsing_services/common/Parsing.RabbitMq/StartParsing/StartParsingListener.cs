using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Parsing.RabbitMq.StartParsing;

internal sealed class StartParsingListener(
    IConnection connection,
    StartParsingListenerOptions options
) : IStartParsingListener
{
    private readonly string _queue = $"{options.ParserName}_{options.ParserType}";
    private IChannel _channel = null!;
    public AsyncEventingBasicConsumer Consumer { get; private set; } = null!;

    public async Task Acknowledge(BasicDeliverEventArgs args, CancellationToken ct = default)
    {
        await _channel.BasicAckAsync(args.DeliveryTag, false, ct);
    }

    public async Task Prepare(CancellationToken ct = default)
    {
        _channel = await connection.CreateChannelAsync(cancellationToken: ct);
        await _channel.ExchangeDeclareAsync(
            RabbitMqScraperConstants.ScrapersExchange,
            ExchangeType.Direct,
            false,
            false,
            null,
            cancellationToken: ct
        );
        await _channel.QueueDeclareAsync(_queue, false, false, false, null, cancellationToken: ct);
        await _channel.QueueBindAsync(
            _queue,
            RabbitMqScraperConstants.ScrapersExchange,
            _queue,
            null,
            cancellationToken: ct
        );
        Consumer = new AsyncEventingBasicConsumer(_channel);
    }

    public async Task StartConsuming(CancellationToken ct = default)
    {
        await _channel.BasicConsumeAsync(
            queue: _queue,
            autoAck: false,
            Consumer,
            cancellationToken: ct
        );
    }
}
