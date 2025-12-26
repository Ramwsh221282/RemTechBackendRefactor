using System.Text;
using System.Text.Json;
using ParsersControl.Core.Contracts;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RabbitMQ.Client;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.Infrastructure.Listeners;

public sealed class OnParserStartedEventListener(
    RabbitMqConnectionSource rabbitMq,
    Serilog.ILogger logger
    ) : IOnParserStartedListener
{
    private Serilog.ILogger Logger { get; } = logger.ForContext<IOnParserStartedListener>();
    
    public async Task Handle(SubscribedParser parser, CancellationToken ct = default)
    {
        Logger.Information("Handling on parser started event.");
        (Guid parserId, string domain, string type, IReadOnlyList<SubscribedParserLink> links) = GetParserInfo(parser);
        (string exchange, string routingKey) = FormPublishingOptions((domain, type));
         Logger.Information("Domain: {Domain}, Type: {Type}, Id: {Id}", domain, type, parserId);
        ReadOnlyMemory<byte> body = CreatePayload(parserId, domain, type, links);
        await PublishMessage(exchange, routingKey, body, ct);
    }
    
    private (Guid parserId, string domain, string type, IReadOnlyList<SubscribedParserLink> links) GetParserInfo(SubscribedParser parser)
    {
        string parserDomain = parser.Identity.DomainName;
        string parserType = parser.Identity.ServiceType;
        Guid parserId = parser.Id.Value;
        IReadOnlyList<SubscribedParserLink> links = parser.Links;
        return (parserId, parserDomain, parserType, links);
    }

    private (string exchange, string routingKey) FormPublishingOptions((string domain, string type) parserInfo)
    {
        string exchange = $"{parserInfo.domain}.{parserInfo.type}";
        string routingKey = $"{exchange}.start";
        return (exchange, routingKey);
    }
    
    private ReadOnlyMemory<byte> CreatePayload(Guid parserId, string domain, string type, IReadOnlyList<SubscribedParserLink> links)
    {
        object payload = new
        {
            parser_id = parserId,
            parser_domain = domain,
            parser_type = type,
            parser_links = links.Select(l => new
            {
                id = l.Id.Value,
                url = l.UrlInfo.Url
            })
        };
        
        string json = JsonSerializer.Serialize(payload);
        return Encoding.UTF8.GetBytes(json);
    }
    
    private async Task PublishMessage(string exchange, string routingKey, ReadOnlyMemory<byte> body, CancellationToken ct = default)
    {
        IConnection connection = await rabbitMq.GetConnection(ct);
        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: ct);
        await channel.BasicPublishAsync(exchange: exchange, routingKey: routingKey, body: body, cancellationToken: ct);
        Logger.Information("Published message to exchange {Exchange}, routing key {RoutingKey}", exchange, routingKey);
    }
}