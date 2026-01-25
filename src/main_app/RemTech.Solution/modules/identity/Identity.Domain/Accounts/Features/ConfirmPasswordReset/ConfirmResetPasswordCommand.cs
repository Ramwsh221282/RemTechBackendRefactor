using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.ConfirmPasswordReset;

public sealed record ConfirmResetPasswordCommand(Guid AccountId, Guid TicketId, string NewPassword) : ICommand;
