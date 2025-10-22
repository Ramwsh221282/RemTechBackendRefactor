using FluentValidation;
using FluentValidation.Results;
using Identity.Domain.Roles;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.UseCases.Common;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.UserDemotesUser;

public sealed class UserDemotesUserHandler(
    IGetVerifiedUserHandle getVerified,
    IGetUserByIdHandle getUserById,
    IGetRoleByIdHandle getRoleById,
    IValidator<UserDemotesUserCommand> validator,
    IDomainEventsDispatcher dispatcher
) : ICommandHandler<UserDemotesUserCommand, Status<IdentityUser>>
{
    public async Task<Status<IdentityUser>> Handle(
        UserDemotesUserCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.ValidationError();

        Status<IdentityUser> demoter = await getVerified.Handle(
            command.DemoterId,
            command.DemoterPassword,
            ct
        );
        if (demoter.IsFailure)
            return demoter.Error;

        Status<IdentityRole> role = await getRoleById.Handle(command.RoleName, ct);
        if (role.IsFailure)
            return role.Error;

        Status<IdentityUser> target = await getUserById.Handle(command.UserId, ct);
        if (target.IsFailure)
            return target.Error;

        Status demotion = demoter.Value.Demote(target, role);
        if (demotion.IsFailure)
            return demotion.Error;

        Status handling = await target.Value.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : target;
    }
}
