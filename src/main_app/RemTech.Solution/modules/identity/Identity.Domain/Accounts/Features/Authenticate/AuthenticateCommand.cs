using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.Authenticate;

public sealed record AuthenticateCommand(string? Login, string? Email, string Password) : ICommand;