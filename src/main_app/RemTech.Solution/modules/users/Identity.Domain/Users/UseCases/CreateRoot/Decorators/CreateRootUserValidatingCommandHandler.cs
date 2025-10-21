using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.CreateRoot.Decorators;

public sealed class CreateRootUserValidatingCommandHandler(
    ICommandHandler<CreateRootUserCommand, Status<User>> handler
) : ICommandHandler<CreateRootUserCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        CreateRootUserCommand command,
        CancellationToken ct = default
    )
    {
        ValidationScope scope = new ValidationScope()
            .Check(UserLogin.Create(command.Name))
            .Check(UserEmail.Create(command.Email))
            .Check(UserPassword.Create(command.Password));

        return scope.Any() ? scope.ToError() : await handler.Handle(command, ct);
    }
}
