using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.ConfirmEmailTicket;

public sealed record ConfirmEmailTicketCommand(Guid TicketId) : ICommand;
