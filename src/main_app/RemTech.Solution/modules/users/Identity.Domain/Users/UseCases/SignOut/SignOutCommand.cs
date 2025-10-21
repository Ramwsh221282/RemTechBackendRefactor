using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.SignOut;

public sealed record SignOutCommand(Guid TokenId) : ICommand;
