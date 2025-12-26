using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.Tests.Parsers.SubscribeParser;

public sealed class FakeParserSubscribeProducer(Serilog.ILogger logger, RabbitMqProducer producer)
{
    private Serilog.ILogger Logger { get; } = logger.ForContext<FakeParserSubscribeProducer>();
    public async Task Publish(Guid parserId, string domain, string type)
    {
        Logger.Information("Publishing parser subscribe message. Id: {Id}, Domain: {Domain}, Type: {Type}", parserId, domain, type);
        SubscribeParserMessage message = new()
        {
            parser_id = parserId,
            parser_domain = domain,
            parser_type = type
        };
        
        await producer.PublishDirectAsync(message, "parsers", "parsers.create", new RabbitMqPublishOptions());
        Logger.Information("Published parser subscribe message.");
    }

    public sealed class SubscribeParserMessage
    {
        public Guid parser_id { get; set; }
        public string parser_domain { get; set; }
        public string parser_type { get; set; }
    }
}