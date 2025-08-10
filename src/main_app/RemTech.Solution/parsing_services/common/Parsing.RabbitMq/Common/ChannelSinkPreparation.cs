using RabbitMQ.Client;

namespace Parsing.RabbitMq.Common;

public sealed class ChannelSinkPreparation(IChannel channel)
{
    public async Task<IChannel> Prepared(string exchange, string queue)
    {
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct, false, false, null);
        await channel.QueueDeclareAsync(queue, false, false, false, null);
        await channel.QueueBindAsync(queue, exchange, queue, null);
        return channel;
    }
}
