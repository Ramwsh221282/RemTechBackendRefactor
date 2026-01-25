using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.ResetPassword;

public sealed record ResetPasswordCommand(string? Login, string? Email) : ICommand;
