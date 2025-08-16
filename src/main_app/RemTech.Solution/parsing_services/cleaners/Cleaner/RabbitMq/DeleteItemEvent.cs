using System.Text;
using System.Text.Json;
using Cleaner.Constants;
using RabbitMQ.Client;

namespace Cleaner.RabbitMq;

internal sealed class DeleteItemEvent(IConnection connection, string id)
{
    private const string Queue = RabbitMqConstants.CleanersCleanItemQueue;
    private const string Exchange = RabbitMqConstants.CleanersExchange;

    public async Task Publish()
    {
        await using IChannel channel = await connection.CreateChannelAsync();
        await channel.BasicPublishAsync(Exchange, Queue, body: MakeBody());
    }

    private ReadOnlyMemory<byte> MakeBody()
    {
        Dictionary<string, object> data = [];
        data.Add("id", id);
        string json = JsonSerializer.Serialize(data);
        return Encoding.UTF8.GetBytes(json);
    }
}
