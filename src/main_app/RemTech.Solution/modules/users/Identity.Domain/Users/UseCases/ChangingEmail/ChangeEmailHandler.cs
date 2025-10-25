using FluentValidation;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.UseCases.Common;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.ChangingEmail;

public sealed class ChangeEmailHandler(
    IUserEmailUnique emailUnique,
    IGetUserByIdHandle getUser,
    IDomainEventsDispatcher eventsHandler,
    IValidator<ChangeEmailCommand> validator
) : ICommandHandler<ChangeEmailCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        ChangeEmailCommand command,
        CancellationToken ct = default
    )
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.ValidationError();

        var newEmail = UserEmail.Create(command.NewEmail);
        var uniquesness = await emailUnique.Unique(newEmail, ct);
        if (uniquesness.IsFailure)
            return uniquesness.Error;

        var user = await getUser.Handle(command.ChangerId, ct);
        if (user.IsFailure)
            return user.Error;

        // incompleted logic.
        throw new NotImplementedException();

        var handling = await user.Value.PublishEvents(eventsHandler, ct);
        return handling.IsFailure ? handling.Error : user;
    }
}
