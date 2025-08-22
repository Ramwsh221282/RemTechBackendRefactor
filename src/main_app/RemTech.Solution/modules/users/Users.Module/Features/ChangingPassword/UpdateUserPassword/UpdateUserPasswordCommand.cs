using Shared.Infrastructure.Module.Cqrs;

namespace Users.Module.Features.ChangingPassword.UpdateUserPassword;

internal sealed record UpdateUserPasswordCommand(
    string InputPassword,
    string NewPassword,
    Guid UserId
) : ICommand;
