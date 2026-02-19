using System.Text.Json;
using RabbitMQ.Client.Events;

namespace ParsingSDK.ParserStopingContext;

public sealed class ParserStopMessage
{
    public required string Domain { get; set; }
    public required string Type { get; set; }    

    public static ParserStopMessage FromDeliverEventArgs(BasicDeliverEventArgs ea)
    {
        ReadOnlyMemory<byte> body = ea.Body;
        return JsonSerializer.Deserialize<ParserStopMessage>(body.Span)!;
    }
}
