using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.ConfirmPasswordReset;

public sealed record ConfirmPasswordResetCommand(Guid TicketId, string NewPassword) : ICommand;
