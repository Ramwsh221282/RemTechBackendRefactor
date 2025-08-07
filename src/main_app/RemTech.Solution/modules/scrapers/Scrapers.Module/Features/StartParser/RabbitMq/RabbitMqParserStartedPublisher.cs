using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Scrapers.Module.Features.StartParser.Models;

namespace Scrapers.Module.Features.StartParser.RabbitMq;

internal sealed class RabbitMqParserStartedPublisher(ConnectionFactory factory)
    : IParserStartedPublisher
{
    private IConnection? _connection;
    private IChannel? _channel;
    private const string Exchange = "scrapers";

    public async Task Publish(StartedParser parser)
    {
        string key = $"{parser.ParserName}_{parser.ParserType}";
        _connection ??= await factory.CreateConnectionAsync();
        _channel ??= await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(Exchange, ExchangeType.Direct, false, false, null);
        await _channel.QueueDeclareAsync(key, false, false, false, null);
        await _channel.QueueBindAsync(key, Exchange, key, null, false);
        ReadOnlyMemory<byte> payload = ParserAsBytes(CreateRabbitMqParser(parser));
        await _channel.BasicPublishAsync(Exchange, key, body: payload);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.DisposeAsync();
        if (_connection != null)
            await _connection.DisposeAsync();
    }

    private static ReadOnlyMemory<byte> ParserAsBytes(ParserStartedRabbitMqMessage parser)
    {
        string json = JsonSerializer.Serialize(parser);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        return bytes;
    }

    private static ParserStartedRabbitMqMessage CreateRabbitMqParser(StartedParser parser)
    {
        IEnumerable<ParserLinkStartedRabbitMqMessage> links = CreateRabbitMqParserLinks(
            parser,
            parser.Links
        );
        return new ParserStartedRabbitMqMessage(
            parser.ParserName,
            parser.ParserType,
            parser.ParserDomain,
            links
        );
    }

    private static IEnumerable<ParserLinkStartedRabbitMqMessage> CreateRabbitMqParserLinks(
        StartedParser parser,
        IEnumerable<StartedParserLink> links
    )
    {
        return links.Select(l => new ParserLinkStartedRabbitMqMessage(
            parser.ParserName,
            parser.ParserType,
            l.LinkName,
            l.LinkUrl
        ));
    }
}
