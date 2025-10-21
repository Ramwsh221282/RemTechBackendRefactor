using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.AdminCreatesUser.Decorators;

public sealed class AdminCreatesUserValidatingCommandHandler(
    ICommandHandler<AdminCreatesUserCommand, Status<User>> handler
) : ICommandHandler<AdminCreatesUserCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        AdminCreatesUserCommand command,
        CancellationToken ct = default
    )
    {
        ValidationScope scope = new ValidationScope()
            .Check(UserId.Create(command.CreatorId))
            .Check(UserLogin.Create(command.NewUserLogin))
            .Check(UserEmail.Create(command.NewUserEmail))
            .Check(UserPassword.Create(command.CreatorPassword));

        return scope.Any() ? scope.ToError() : await handler.Handle(command, ct);
    }
}
