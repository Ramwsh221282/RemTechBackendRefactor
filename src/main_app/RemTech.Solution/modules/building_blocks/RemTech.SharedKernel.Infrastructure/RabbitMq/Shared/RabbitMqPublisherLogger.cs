namespace RemTech.SharedKernel.Infrastructure.RabbitMq.Shared;

public sealed class RabbitMqPublisherLogger(Serilog.ILogger logger)
{
    public void Log(string message, string queue, string exchange, string routingKey)
    {
        object[] logProperties = [message, queue, exchange, routingKey];
        logger.Information("""
                           Publishing message: 
                           Body: {Body}
                           Queue: {Queue}
                           Exchange: {Exchange}
                           RoutingKey: {RoutingKey}
                           """, logProperties);
    }
}