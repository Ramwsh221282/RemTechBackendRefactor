using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Cleaners.Module.RabbitMq;

internal sealed class StartCleaningPublisher(ConnectionFactory factory, Serilog.ILogger logger)
{
    private const string Exchange = RabbitMqConstants.CleanersExchange;
    private const string Queue = RabbitMqConstants.CleanersStartQueue;

    public async Task Publish(StartCleaningMessage message, CancellationToken ct = default)
    {
        await using IConnection connection = await factory.CreateConnectionAsync(ct);
        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: ct);
        await channel.ExchangeDeclareAsync(
            Exchange,
            ExchangeType.Direct,
            false,
            false,
            cancellationToken: ct
        );
        await channel.QueueDeclareAsync(Queue, false, false, false, cancellationToken: ct);
        await channel.QueueBindAsync(Queue, Exchange, Queue, cancellationToken: ct);
        ReadOnlyMemory<byte> payload = MessageAsBytes(message);
        await channel.BasicPublishAsync(Exchange, Queue, payload, ct);
        logger.Information(
            "Start clean items message sent. Items amount: {Count}.",
            message.Items.Count
        );
    }

    private static ReadOnlyMemory<byte> MessageAsBytes(StartCleaningMessage message)
    {
        string json = JsonSerializer.Serialize(message);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        return bytes;
    }
}
