using FluentValidation;
using FluentValidation.Results;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Ports.EventHandlers;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.ChangingEmail;

public sealed class ChangeEmailHandler(
    IUsersStorage users,
    IIdentityUserEventHandler eventsHandler,
    IValidator<ChangeEmailCommand> validator
) : ICommandHandler<ChangeEmailCommand, Status<IdentityUser>>
{
    public async Task<Status<IdentityUser>> Handle(
        ChangeEmailCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.ValidationError();

        UserId id = UserId.Create(command.ChangerId);
        UserEmail newEmail = UserEmail.Create(command.NewEmail);

        IdentityUser? user = await users.Get(id, ct);
        if (user == null)
            return Error.NotFound("Пользователь не найден");

        user.ChangeEmail(newEmail);
        Status handling = await user.PublishEvents(eventsHandler, ct);
        return handling.IsFailure ? handling.Error : user;
    }
}
