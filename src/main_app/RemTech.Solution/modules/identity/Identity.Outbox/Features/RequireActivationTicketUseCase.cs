using System.Text.Json;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Domain.Tickets;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;
using RemTech.Outbox.Shared;

namespace Identity.Outbox.Features;

public static class RequireActivationTicketUseCase
{
    private const string Queue = "tickets";
    private const string Exchange = "tickets";
    private const string RoutingKey = "tickets.create";
    private const string Type = "require.activation.ticket";
    
    public static RequireActivationTicket WithOutboxListener(
        RequireActivationTicket origin,
        NpgSqlSession session,
        OutboxServicesRegistry registry) =>
        async args =>
        {
            CancellationToken ct = args.Ct;
            Result<RequireActivationTicketResult> result = await origin(args);
            if (result.IsFailure) return result.Error;
            string json = CreateJsonBody(result);
            OutboxMessage message = OutboxMessage.New(Queue, Exchange, RoutingKey, Type, json);
            OutboxService service = registry.GetService(session, "identity_module");
            await service.Add(message, ct);
            return result;
        };
    
    private static string CreateJsonBody(RequireActivationTicketResult ticket)
    {
        
        SubjectTicketSnapshot ticketSnap = ticket.Ticket.Snapshot();
        SubjectSnapshot subjectSnap = ticket.Subject.Snapshot();
        object body = new
        {
            creator_id = ticketSnap.CreatorId.Value, 
            ticket_id = ticketSnap.Id, 
            type = Type,
            extra = new
            {
                confirmation_email = subjectSnap.Email
            }
        };
        
        return JsonSerializer.Serialize(body);
    }

    extension(RequireActivationTicket origin)
    {
        public RequireActivationTicket WithOutboxListener(IServiceProvider sp)
        {
            OutboxServicesRegistry messages = sp.Resolve<OutboxServicesRegistry>();
            NpgSqlSession session = sp.Resolve<NpgSqlSession>();
            return WithOutboxListener(origin, session, messages);
        }
    }
}