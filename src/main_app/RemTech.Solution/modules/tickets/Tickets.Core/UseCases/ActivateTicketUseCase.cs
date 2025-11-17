using Tickets.Core.Contracts;

namespace Tickets.Core.UseCases;

public static class ActivateTicketUseCase
{
    public static ActivateTicket ActivateTicket => args =>
    {
        if (args.Ticket.NoValue) return Task.FromResult<Result<Ticket>>(NotFound("Заявка не найдена."));
        Ticket ticket = args.Ticket.Value;
        Result<Ticket> activated = ticket.ActivateBy(args.CreatorId);
        if (activated.IsFailure) return Task.FromResult<Result<Ticket>>(activated.Error);
        return Task.FromResult(activated);
    };
}