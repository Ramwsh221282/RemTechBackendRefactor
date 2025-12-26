using Identity.Gateways.AccountTickets.OnAccountTicketActivationRequired;
using Identity.Gateways.Common;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTech.SharedKernel.Infrastructure.RabbitMq.Publishers;
using RemTech.SharedKernel.Infrastructure.RabbitMq.Shared;

namespace Identity.Gateways.AccountTickets.RabbitMq;

public sealed class OnAccountActivationRequiredRabbitMqPublisher(
    RabbitMqConnectionSource connectionSource,
    RabbitMqPublisherLogger logger,
    RabbitMqPublishingMethod method) 
    : IRabbitMqOutboxMessagePublisher
{
    private const string Queue = IdentityRabbitMqConstants.Queue;
    private const string Exchange = IdentityRabbitMqConstants.Exchange;
    private const string RoutingKey = AddAccountTicketOnAccountActivationRequested.Type;
    
    public string SupportedMessageType => RoutingKey;
    
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
}