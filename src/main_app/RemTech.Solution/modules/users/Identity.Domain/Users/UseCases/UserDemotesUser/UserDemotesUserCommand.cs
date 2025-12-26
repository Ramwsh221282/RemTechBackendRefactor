using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.UserDemotesUser;

public sealed record UserDemotesUserCommand(
    Guid DemoterId,
    Guid UserId,
    string DemoterPassword,
    string RoleName
) : ICommand;
