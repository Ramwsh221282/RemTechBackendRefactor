using Identity.Contracts.AccountTickets;
using Identity.Contracts.AccountTickets.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.Outbox;

public sealed class OutboxOnAccountTicketCreatedEventListener(
    Serilog.ILogger logger,
    IdentityOutboxMessagesStore messages
    ) : IOnAccountTicketCreatedEventListener
{
    private const string context = nameof(OutboxOnAccountTicketCreatedEventListener);
    
    public async Task<Unit> React(AccountTicketData data, CancellationToken ct = default)
    {
        logger.Information("{Context} reacting on account ticket creation.", context);
        const bool wasSent = false;
        IdentityOutboxMessage message = new(Guid.NewGuid(), data.Type, data.Payload, DateTime.UtcNow, wasSent);
        await messages.Add(message, ct);
        logger.Information("{Context} reacted on account ticket creation.", context);
        return Unit.Value;
    }
}