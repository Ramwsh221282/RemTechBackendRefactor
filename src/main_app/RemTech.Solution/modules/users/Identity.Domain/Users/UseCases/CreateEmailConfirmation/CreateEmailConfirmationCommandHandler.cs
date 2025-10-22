using FluentValidation;
using FluentValidation.Results;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.CreateEmailConfirmation;

public sealed class CreateEmailConfirmationCommandHandler(
    IUsersStorage users,
    IValidator<CreateEmailConfirmationCommand> validator,
    IDomainEventsDispatcher dispatcher
) : ICommandHandler<CreateEmailConfirmationCommand, Status<IdentityUser>>
{
    public async Task<Status<IdentityUser>> Handle(
        CreateEmailConfirmationCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.ValidationError();

        UserId id = UserId.Create(command.UserId);
        IdentityUser? user = await users.Get(id, ct);
        if (user == null)
            return Error.NotFound("Пользователь не найден.");

        Status creating = user.FormEmailConfirmationToken();
        if (creating.IsFailure)
            return creating.Error;

        Status handling = await user.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : user;
    }
}
