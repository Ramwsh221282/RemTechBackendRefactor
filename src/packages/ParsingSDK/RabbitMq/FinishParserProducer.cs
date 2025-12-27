using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsingSDK.RabbitMq;

public sealed class FinishParserProducer(RabbitMqConnectionSource rabbitMq, Serilog.ILogger logger)
{
    private const string Exchange = "parsers";
    private const string RoutingKey = "parser.finish";
    
    public async Task Publish(FinishParserMessage message, CancellationToken ct = default)
    {
        RabbitMqProducer producer = new RabbitMqProducer(logger, rabbitMq);
        RabbitMqPublishOptions options = new RabbitMqPublishOptions() { Persistent = true };
        await producer.PublishDirectAsync(message, Exchange, RoutingKey, options, ct);
    }
}