using Shared.Infrastructure.Module.Cqrs;
using Users.Module.Models;

namespace Users.Module.Features.ChangingEmail;

internal sealed record UpdateUserEmailCommand(
    UserJwt UserJwt,
    string NewEmail,
    string InputPassword,
    Guid UserId
) : ICommand;
