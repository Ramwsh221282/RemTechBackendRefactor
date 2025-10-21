using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.RootCreatesAdmin.Decorators;

public sealed class RootCreatesAdminValidatingHandler(
    ICommandHandler<RootCreatesAdminCommand, Status<User>> handler
) : ICommandHandler<RootCreatesAdminCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        RootCreatesAdminCommand command,
        CancellationToken ct = default
    )
    {
        ValidationScope scope = new ValidationScope()
            .Check(UserLogin.Create(command.NewUserLogin))
            .Check(UserEmail.Create(command.NewUserEmail))
            .Check(UserId.Create(command.CreatorId))
            .Check(UserPassword.Create(command.CreatorPassword));

        return scope.Any() ? scope.ToError() : await handler.Handle(command, ct);
    }
}
