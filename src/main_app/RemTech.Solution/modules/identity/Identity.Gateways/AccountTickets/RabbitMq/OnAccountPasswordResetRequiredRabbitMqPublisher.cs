using Identity.Gateways.AccountTickets.OnAccountTicketPasswordResetRequired;
using Identity.Gateways.Common;

namespace Identity.Gateways.AccountTickets.RabbitMq;

public sealed class OnAccountPasswordResetRequiredRabbitMqPublisher(
    RabbitMqConnectionSource connectionSource,
    RabbitMqPublisherLogger logger,
    RabbitMqPublishingMethod method
) : IRabbitMqOutboxMessagePublisher
{
    private const string Queue = IdentityRabbitMqConstants.Queue;
    private const string Exchange = IdentityRabbitMqConstants.Exchange;
    private const string RoutingKey = AddAccountTicketOnAccountPasswordResetRequired.Type;
    
    public async Task<PublishDeliveryInfo> Publish(string message, CancellationToken ct = default)
    {
        PublishDeliveryInfo delivery = await method.Publish(
            connectionSource, 
            message, 
            Queue, 
            Exchange, 
            RoutingKey, 
            ct);
        logger.Log(message, Queue, Exchange, RoutingKey);
        return delivery;
    }

    public string SupportedMessageType => RoutingKey;
}