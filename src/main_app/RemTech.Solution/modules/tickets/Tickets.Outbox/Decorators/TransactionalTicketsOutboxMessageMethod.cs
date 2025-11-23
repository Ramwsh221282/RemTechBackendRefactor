using RemTech.NpgSql.Abstractions;
using RemTech.Outbox.Shared;

namespace Tickets.Outbox.Decorators;

public sealed class TransactionalTicketsOutboxMessageMethod(
    NpgSqlSession session,
    ITicketsOutboxJobMethod origin) : ITicketsOutboxJobMethod
{
    public  async Task<ProcessedOutboxMessages> ProcessMessages(CancellationToken ct)
    {
        await session.GetTransaction(ct);
        ProcessedOutboxMessages result = await origin.ProcessMessages(ct);
        return !await session.Commited(ct)
            ? throw new ApplicationException("Unable to commit tickets outbox transaction.")
            : result;
    }
}