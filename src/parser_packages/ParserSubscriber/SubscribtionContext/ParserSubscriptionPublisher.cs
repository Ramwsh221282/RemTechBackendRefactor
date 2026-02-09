using Microsoft.Extensions.Options;
using ParserSubscriber.SubscribtionContext.Options;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Serilog;

namespace ParserSubscriber.SubscribtionContext;

public sealed class ParserSubscriptionPublisher(
    RabbitMqConnectionSource rabbitMq,
    IOptions<RabbitMqRequestReplyResponseListeningQueueOptions> options,
    ILogger? logger)
{
    private const string Exchange = "parsers";
    private const string RoutingKey = "parsers.create";
    private ILogger? Logger { get; } = logger;

    private sealed class ParserCreateMessage
    {
        public Guid parser_id { get; set; }
        public string parser_type { get; set; }
        public string parser_domain { get; set; }
    }
    
    internal async Task Publish(ParserSubscribtion subscription)
    {
        Logger?.Information("Publishing subscribe parser message");
        try
        {
            RabbitMqProducer producer = new RabbitMqProducer(Logger, rabbitMq);
            ParserCreateMessage payload = new() { parser_id = subscription.Id, parser_type = subscription.Type, parser_domain = subscription.Domain };
            await producer.PublishDirectAsync(payload, Exchange, RoutingKey, new RabbitMqPublishOptions() { Persistent = true });
            Logger?.Information("Published subscribe parser message");
        }
        catch(Exception ex)
        {
            Logger?.Error(ex, "Publishing subscribe parser message resulted in error.");
            throw;
        }
    }
}