using System.Text;
using System.Text.Json;
using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RabbitMQ.Client;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.Infrastructure.Listeners;

public sealed class OnParserSubscribedEventListener(
    RabbitMqConnectionSource rabbitMq,
    Serilog.ILogger logger) : IOnParserSubscribedListener
{
    private Serilog.ILogger Logger { get; } = logger.ForContext<IOnParserSubscribedListener>();
    
    public async Task Handle(SubscribedParser parser, CancellationToken ct = default)
    {
        Logger.Information("Handling on parser subscribed.");    
        (string domain, string type) = GetParserInfo(parser);
        (string exchange, string queue, string routingKey) = FormPublishingOptions(domain, type);
        Logger.Information("Domain: {Domain}, Type: {Type}", domain, type);
        Logger.Information("Exchange: {Exchange}, Queue: {Queue}, RoutingKey: {RoutingKey}", exchange, queue, routingKey);
        ReadOnlyMemory<byte> body = CreatePayload(parser.Id.Value, domain, type);
        await PublishMessage(queue, exchange, routingKey, body, ct);
    }
    
    private (string domain, string type) GetParserInfo(SubscribedParser parser)
    {
        string parserDomain = parser.Identity.DomainName;
        string parserType = parser.Identity.ServiceType;
        return (parserDomain, parserType);
    }
    
    private (string exchange, string queue, string routingKey) FormPublishingOptions(string domain, string type)
    {
        string exchange = $"{domain}.{type}";
        string queue = $"{exchange}.confirmation";
        string routingKey = $"{exchange}.confirmation";
        return (exchange, queue, routingKey);
    }
    
    private ReadOnlyMemory<byte> CreatePayload(Guid parserId, string domain, string type)
    {
        object payload = new { parser_id = parserId, parser_domain = domain, parser_type = type };
        string json = JsonSerializer.Serialize(payload);
        return Encoding.UTF8.GetBytes(json);
    }
    
    private async Task PublishMessage(string queue, string exchange, string routingKey, ReadOnlyMemory<byte> body, CancellationToken ct)
    {
        IConnection connection = await rabbitMq.GetConnection(ct);
        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: ct);
        await channel.ExchangeDeclareAsync(exchange: exchange, durable: true, autoDelete: false, type: "topic");
        await channel.QueueDeclareAsync(queue: queue, durable: false, autoDelete: false, exclusive: false);
        await channel.QueueBindAsync(queue: queue, exchange: exchange, routingKey: routingKey);
        Logger.Information("Published message to exchange {Exchange}, routing key {RoutingKey}", exchange, routingKey);
    }
}