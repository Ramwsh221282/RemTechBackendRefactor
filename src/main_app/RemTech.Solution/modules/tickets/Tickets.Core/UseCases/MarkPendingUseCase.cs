using Tickets.Core.Contracts;

namespace Tickets.Core.UseCases;

public static class MarkPendingUseCase
{
    public static MarkPending MarkPending => args =>
    {
        if (args.Target.NoValue) return Task.FromResult<Result<Ticket>>(NotFound("Заявка не найдена"));
        Ticket ticket = args.Target.Value;
        Result<Ticket> pending = ticket.MarkPending();
        if (pending.IsFailure) return Task.FromResult<Result<Ticket>>(pending.Error);
        return Task.FromResult(pending);
    };
}