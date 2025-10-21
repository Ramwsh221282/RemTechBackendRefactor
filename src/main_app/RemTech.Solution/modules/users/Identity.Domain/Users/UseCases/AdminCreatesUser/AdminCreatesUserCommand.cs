using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.AdminCreatesUser;

public sealed record AdminCreatesUserCommand(
    Guid CreatorId,
    string CreatorPassword,
    string NewUserLogin,
    string NewUserEmail
) : ICommand;
