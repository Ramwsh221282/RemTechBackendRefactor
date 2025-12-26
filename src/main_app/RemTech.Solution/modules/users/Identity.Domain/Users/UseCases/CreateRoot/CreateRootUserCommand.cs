using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.CreateRoot;

public sealed record CreateRootUserCommand(string Email, string Name, string Password) : ICommand;
