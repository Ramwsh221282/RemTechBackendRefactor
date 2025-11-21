using System.Text.Json;
using RemTech.Functional.Extensions;
using RemTech.RabbitMq.Abstractions.Publishers;
using RemTech.RabbitMq.Abstractions.Publishers.Topic;
using Tickets.Core.Contracts;
using Tickets.Core.Snapshots;

namespace Tickets.EventListeners.Routers.RequireActivationTicket;

public sealed class RequireActivationTicketRouter : ITicketRouter
{
    private const string Queue = "mailing";
    private const string Exchange = "mailing";
    private const string RoutingKey = "mailing.send.email";
    private const string Type = "mailing.send.email";
    private readonly RabbitMqPublishers _publishers;
    public string SupportedTicketType { get; } = "require.activation.ticket";
    
    public async Task<TicketRoutingResult> RoutingMethod(TicketRouting routing)
    {
        if (!routing.Ticket.OfType(Type))
            return new TicketRoutingResult("Ticket type is not supported.", false);
        
        TicketSnapshot snapshot = routing.Ticket.Snapshot();
        JsonRequiredActivationTicketEmail email = new(snapshot.Metadata.Extra);
        Result<string> emailString = email.Email();
        if (emailString.IsFailure) return new TicketRoutingResult(emailString.Error.Message, emailString.IsSuccess);
        
        object content = new ActivationEmailMessage(snapshot.Metadata.TicketId.ToString()).Content();        
        TopicPublishOptions options = new(JsonSerializer.Serialize(content), Queue, Exchange, RoutingKey);
        IRabbitMqPublisher<TopicPublishOptions> publisher = await _publishers.TopicPublisher(routing.Ct);
        await publisher.Publish(options, routing.Ct);
        return new TicketRoutingResult("", true);
    }

    public RequireActivationTicketRouter(RabbitMqPublishers publishers)
    {
        _publishers = publishers;
    }
}