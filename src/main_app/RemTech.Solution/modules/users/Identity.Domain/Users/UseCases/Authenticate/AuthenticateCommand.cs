using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.Authenticate;

public sealed record AuthenticateCommand(string? Login, string? Email, string Password) : ICommand;
