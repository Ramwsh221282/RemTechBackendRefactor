using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.ChangePassword;

public sealed record ChangePasswordCommand(
    string AccessToken,
    string RefreshToken,
    Guid Id,
    string NewPassword,
    string CurrentPassword
) : ICommand;
