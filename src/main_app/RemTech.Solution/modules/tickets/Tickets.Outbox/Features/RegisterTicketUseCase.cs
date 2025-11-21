using System.Text.Json;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;
using RemTech.Outbox.Shared;
using Tickets.Core;
using Tickets.Core.Contracts;
using Tickets.Core.Snapshots;

namespace Tickets.Outbox.Features;

public static class RegisterTicketUseCase
{
    private const string Queue = "mailing";
    private const string Exchange = "mailing";
    private const string RoutingKey = "mailing.send.email";
    
    private static RegisterTicket RegisterTicket(OutboxService outbox, RegisterTicket origin) => async (args) =>
    {
        Result<Ticket> result = await origin(args);
        if (result.IsFailure) return result.Error;
        
        TicketSnapshot snapshot = result.Value.Snapshot();
        TicketMetadataSnapshot metadata = snapshot.Metadata;
        if (string.IsNullOrWhiteSpace(metadata.Extra)) return result;
        
        string messageBody = MessageBody(metadata);
        OutboxMessage message = OutboxMessage.New(Queue, Exchange, RoutingKey, metadata.Type, messageBody);
        await outbox.Add(message, args.Ct);
        return result;
    };

    private static string MessageBody(TicketMetadataSnapshot metadata)
    {
        object body = new
        {
            ticket_id = metadata.TicketId,
            extra = metadata.Extra,
            type = metadata.Type,
        };
        
        return JsonSerializer.Serialize(body);
    }

    extension(RegisterTicket origin)
    {
        public RegisterTicket WithOutboxHandle(IServiceProvider sp)
        {
            NpgSqlSession session = sp.Resolve<NpgSqlSession>();
            OutboxServicesRegistry registry = sp.Resolve<OutboxServicesRegistry>();
            OutboxService outbox = registry.GetService(session, "tickets_module");
            return RegisterTicket(outbox, origin);
        }
    }
}