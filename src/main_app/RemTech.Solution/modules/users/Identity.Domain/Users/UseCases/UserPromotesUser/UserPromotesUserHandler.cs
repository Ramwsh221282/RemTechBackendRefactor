using FluentValidation;
using FluentValidation.Results;
using Identity.Domain.Roles;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.UserPromotesUser;

public sealed class UserPromotesUserHandler(
    IUsersStorage users,
    IRolesStorage roles,
    IPasswordManager passwordManager,
    IValidator<UserPromotesUserCommand> validator,
    IDomainEventsDispatcher dispatcher
) : ICommandHandler<UserPromotesUserCommand, Status<IdentityUser>>
{
    public async Task<Status<IdentityUser>> Handle(
        UserPromotesUserCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.ValidationError();

        UserId promoterId = UserId.Create(command.PromoterId);
        UserPassword promoterPassword = UserPassword.Create(command.PromoterPassword);
        IdentityUser? promoter = await users.Get(promoterId, ct);
        if (promoter == null)
            return Error.NotFound("Вызывающий пользователь не найден.");

        Status verification = promoter.Verify(promoterPassword, passwordManager);
        if (verification.IsFailure)
            return verification.Error;

        RoleName roleName = RoleName.Create(command.RoleName);
        IdentityRole? requestedRole = await roles.Get(roleName, ct);
        if (requestedRole == null)
            return Error.NotFound($"Несуществующая роль: {roleName.Value}");

        UserId toPromoteId = UserId.Create(command.UserId);
        IdentityUser? toPromote = await users.Get(toPromoteId, ct);
        if (toPromote == null)
            return Error.NotFound("Пользователь не найден.");

        Status promotion = promoter.Promote(toPromote, requestedRole);
        if (promotion.IsFailure)
            return promotion.Error;

        Status handling = await toPromote.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : toPromote;
    }
}
