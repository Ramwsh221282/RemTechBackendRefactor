using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Tokens;
using Identity.Domain.Users.UseCases.UserRegistrationByAdmin.Input;
using Identity.Domain.Users.UseCases.UserRegistrationByAdmin.Output;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.UserRegistrationByAdmin.Handlers;

public sealed class UserRegistrationByAdminValidatingCommandHandler(
    ICommandHandler<UserRegistrationByAdminCommand, Status<UserRegistrationByAdminResponse>> handler
) : ICommandHandler<UserRegistrationByAdminCommand, Status<UserRegistrationByAdminResponse>>
{
    public async Task<Status<UserRegistrationByAdminResponse>> Handle(
        UserRegistrationByAdminCommand command,
        CancellationToken ct = default
    )
    {
        ValidationScope scope = new ValidationScope()
            .Check(Token.Create(command.AdminToken))
            .Check(RoleId.Create(command.RoleId))
            .Check(UserLogin.Create(command.Name))
            .Check(UserEmail.Create(command.Email));

        return scope.Any() ? scope.ToError() : await handler.Handle(command, ct);
    }
}
