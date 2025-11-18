using System.Text.Json;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Tickets;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Outbox.Features;

public static class RequireActivationTicketUseCase
{
    private const string Type = "require.activation.ticket";
    private const string Queue = "tickets";
    private const string RoutingKey = "tickets.create";
    
    public static RequireActivationTicket WithOutboxListener(
        RequireActivationTicket origin,
        IdentityOutboxStorage messages) =>
        async args =>
        {
            CancellationToken ct = args.Ct;
            Result<SubjectTicket> result = await origin(args);
            if (result.IsFailure) return result.Error;
            
            string json = CreateJsonBody(result);
            IdentityOutboxMessage message = IdentityOutboxMessage.New(Type, Queue, RoutingKey, json);
            await messages.Add(message, ct);
            return result;
        };
    
    private static string CreateJsonBody(SubjectTicket ticket)
    {
        SubjectTicketSnapshot ticketSnap = ticket.Snapshot();
        object body = new
        {
            creator_id = ticketSnap.CreatorId, 
            ticket_id = ticketSnap.Id, 
            type = Type
        };
        
        return JsonSerializer.Serialize(body);
    }

    extension(RequireActivationTicket origin)
    {
        public RequireActivationTicket WithOutboxListener(IServiceProvider sp)
        {
            IdentityOutboxStorage messages = sp.Resolve<IdentityOutboxStorage>();
            return WithOutboxListener(origin, messages);
        }
    }
}