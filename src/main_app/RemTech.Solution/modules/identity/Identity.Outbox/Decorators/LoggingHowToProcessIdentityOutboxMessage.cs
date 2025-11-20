using RemTech.Outbox.Shared;

namespace Identity.Outbox.Decorators;

public sealed class LoggingHowToProcessIdentityOutboxMessage : IHowToProcessIdentityOutboxMessage
{
    private readonly Serilog.ILogger _logger;
    private readonly IHowToProcessIdentityOutboxMessage _origin;
    
    public async Task ProcessMessage(ProcessedOutboxMessages messages, OutboxMessage message, CancellationToken ct = default)
    {
        await _origin.ProcessMessage(messages, message, ct);
        
        string body = message.Body;
        string queue = message.Queue;
        string exchange = message.Exchange;
        string routingKey = message.RoutingKey;
            
        _logger.Information("""
                            {Context} message publish info.
                            Body: {Body}
                            Queue: {Queue}
                            Exchange {Exchange}
                            RoutingKey: {RoutingKey}
                            """, nameof(IIdentityOutboxProcessorWork), body, queue, exchange, routingKey);
    }

    public LoggingHowToProcessIdentityOutboxMessage(Serilog.ILogger logger, IHowToProcessIdentityOutboxMessage origin)
    {
        _logger = logger;
        _origin = origin;
    }
}