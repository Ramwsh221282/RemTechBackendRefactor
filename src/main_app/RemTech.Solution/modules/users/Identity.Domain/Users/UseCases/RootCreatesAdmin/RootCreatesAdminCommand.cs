using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.RootCreatesAdmin;

public sealed record RootCreatesAdminCommand(
    Guid CreatorId,
    string CreatorPassword,
    string NewUserLogin,
    string NewUserEmail
) : ICommand;
