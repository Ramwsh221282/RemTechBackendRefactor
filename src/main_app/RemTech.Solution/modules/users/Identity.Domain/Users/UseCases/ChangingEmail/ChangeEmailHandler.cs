using FluentValidation;
using FluentValidation.Results;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.ChangingEmail;

public sealed class ChangeEmailHandler(
    IUsersStorage users,
    IDomainEventsDispatcher eventsHandler,
    IValidator<ChangeEmailCommand> validator
) : ICommandHandler<ChangeEmailCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        ChangeEmailCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.ValidationError();

        UserId id = UserId.Create(command.ChangerId);
        UserEmail newEmail = UserEmail.Create(command.NewEmail);

        User? user = await users.Get(id, ct);
        if (user == null)
            return Error.NotFound("Пользователь не найден");

        throw new NotImplementedException();

        // user.ChangeEmail(newEmail);
        // Status handling = await user.PublishEvents(eventsHandler, ct);
        // return handling.IsFailure ? handling.Error : user;
    }
}
