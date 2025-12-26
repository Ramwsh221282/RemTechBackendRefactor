using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.CreateEmailConfirmationTicket;

public sealed record CreateEmailConfirmationTicketCommand(Guid UserId, string Password) : ICommand;
