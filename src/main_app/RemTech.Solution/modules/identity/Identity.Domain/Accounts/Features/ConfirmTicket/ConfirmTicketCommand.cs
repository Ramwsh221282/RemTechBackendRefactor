using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.ConfirmTicket;

public sealed record ConfirmTicketCommand(Guid AccountId, Guid TicketId) : ICommand;