using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Cleaner.Constants;
using RabbitMQ.Client;

namespace Cleaner.RabbitMq;

internal sealed class JobFinishedEvent(IConnection connection, Stopwatch stopwatch)
{
    private const string Exchange = RabbitMqConstants.CleanersExchange;
    private const string Queue = RabbitMqConstants.CleanersFinishQueue;

    public async Task Publish()
    {
        if (!stopwatch.IsRunning)
            return;
        stopwatch.Stop();
        await using IChannel channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(Exchange, ExchangeType.Direct, false, false);
        await channel.QueueDeclareAsync(Queue, false, false, false);
        await channel.QueueBindAsync(Queue, Exchange, Queue);
        await channel.BasicPublishAsync(Exchange, Queue, body: MakeBody());
    }

    private ReadOnlyMemory<byte> MakeBody()
    {
        Dictionary<string, object> data = [];
        long elapsed = (long)stopwatch.Elapsed.TotalSeconds;
        data.Add("elapsed", elapsed);
        string json = JsonSerializer.Serialize(data);
        return Encoding.UTF8.GetBytes(json);
    }
}
