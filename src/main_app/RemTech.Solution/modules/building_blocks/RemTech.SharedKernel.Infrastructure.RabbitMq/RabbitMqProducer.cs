using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public sealed class RabbitMqProducer(RabbitMqConnectionSource connectionSource)
{
    private IChannel? Channel { get; set; }
    private SemaphoreSlim Semaphore { get; } = new(1);

    public async Task PublishDirectAsync<T>(
        T message,
        string exchange,
        string routingKey,
        RabbitMqPublishOptions options,
        CancellationToken ct = default
    )
    {
        await Semaphore.WaitAsync().ConfigureAwait(false);

        try
        {
            IConnection connection = await connectionSource.GetConnection(ct);
            IChannel channel = await GetChannel(connection, ct);
            string jsonMessage = JsonSerializer.Serialize(message);
            ReadOnlyMemory<byte> body = Encoding.UTF8.GetBytes(jsonMessage);
            BasicProperties properties = new BasicProperties() { Persistent = options.Persistent };
            
            await channel.BasicPublishAsync(
                exchange: exchange, 
                routingKey: routingKey, 
                basicProperties: properties, 
                cancellationToken: ct, 
                body: body, 
                mandatory: true);
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private ValueTask<IChannel> GetChannel(IConnection connection, CancellationToken ct)
    {
        if (Channel is not null && Channel.IsOpen) return new ValueTask<IChannel>(Channel);
        
        async ValueTask<IChannel> CreateAsync()
        {
            IChannel channel = await connection.CreateChannelAsync(cancellationToken: ct);
            Channel = channel;
            return channel;
        }
        
        return CreateAsync();
    }
}