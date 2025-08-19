using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Parsing.RabbitMq.AddJournalRecord;

internal sealed class AddJournalRecordPublisher(IConnection connection) : IAddJournalRecordPublisher
{
    private const string Exchange = RabbitMqScraperConstants.ScrapersExchange;
    private const string Queue = RabbitMqScraperConstants.AddJournalRecordQueue;

    public async Task PublishJournalRecord(
        string parserName,
        string parserType,
        string action,
        string text
    )
    {
        ReadOnlyMemory<byte> body = MakeBody(parserName, parserType, action, text);
        await using IChannel channel = await connection.CreateChannelAsync();
        try
        {
            await channel.BasicPublishAsync(Exchange, Queue, body);
        }
        catch
        {
            // ignored
        }
    }

    private static ReadOnlyMemory<byte> MakeBody(
        string parserName,
        string parserType,
        string action,
        string text
    )
    {
        Dictionary<string, object> data = [];
        data.Add("parserName", parserName);
        data.Add("parserType", parserType);
        data.Add("action", action);
        data.Add("text", text);
        string json = JsonSerializer.Serialize(data);
        ReadOnlyMemory<byte> bytes = Encoding.UTF8.GetBytes(json);
        return bytes;
    }
}
