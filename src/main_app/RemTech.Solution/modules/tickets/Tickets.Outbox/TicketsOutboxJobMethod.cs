using RemTech.Outbox.Shared;
using Tickets.Core.Contracts;

namespace Tickets.Outbox;

public sealed class TicketsOutboxJobMethod(
    TicketRoutersRegistry registry, 
    OutboxService service,
    Serilog.ILogger logger) : ITicketsOutboxJobMethod
{
    private const string Context = nameof(TicketsOutboxJobMethod);
    private readonly ProcessedOutboxMessages _messages = new();
    
    public async Task<ProcessedOutboxMessages> ProcessMessages(CancellationToken ct)
    {
        OutboxMessage[] messages = (await service.GetPendingMessages(50, ct)).ToArray();
        logger.Information("{Context} received messages: {Count}", Context, messages.Length);

        foreach (OutboxMessage message in messages)
        {
            Guid id = message.Id;
            string type = message.Type;
            string body = message.Body;
            object[] logProperties = [Context, id, type, body];

            logger.Information("""
                               {Context} message info:
                               Id: {Id}
                               Type: {Type}
                               Body: {Body}
                               """, logProperties);
            
            _messages.Add(message);
        }

        return _messages;
    }
}