using RemTech.RabbitMq.Abstractions.Publishers;
using RemTech.RabbitMq.Abstractions.Publishers.Topic;
using Tickets.Core;
using Tickets.Core.Contracts;

namespace Tickets.EventListeners.Routers;

public sealed class RequireActivationTicketRouter : ITicketRouter
{
    private const string Queue = "mailing";
    private const string Exchange = "mailing";
    private const string RoutingKey = "mailing.send.email";
    private const string Type = "mailing.send.email";
    
    private readonly IRabbitMqPublisher<TopicPublishOptions> _publisher;
    public string SupportedTicketType { get; } = "require.activation.ticket";
    
    public async Task RouteTicket(Ticket ticket, CancellationToken ct = default)
    {
        
    }
}