using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.ConfirmUserEmail;

public sealed record ConfirmUserEmailCommand(Guid TicketId) : ICommand;
