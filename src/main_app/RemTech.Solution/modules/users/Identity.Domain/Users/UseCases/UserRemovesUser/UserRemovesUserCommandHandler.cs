using FluentValidation;
using FluentValidation.Results;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.UserRemovesUser;

public sealed class UserRemovesUserCommandHandler(
    IUsersStorage users,
    IDomainEventsDispatcher handler,
    IValidator<UserRemovesUserCommand> validator
) : ICommandHandler<UserRemovesUserCommand, Status<IdentityUser>>
{
    public async Task<Status<IdentityUser>> Handle(
        UserRemovesUserCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.ValidationError();

        UserId removerId = UserId.Create(command.RemoverId);
        UserId removalId = UserId.Create(command.RemovalId);

        IdentityUser? remover = await users.Get(removerId, ct);
        if (remover == null)
            return Error.NotFound("Удаляющий пользователь не найден.");

        IdentityUser? toRemove = await users.Get(removalId, ct);
        if (toRemove == null)
            return Error.NotFound("Удаляемый пользователь не найден.");

        Status removing = remover.RemoveUser(toRemove);
        if (removing.IsFailure)
            return removing.Error;

        Status handling = await remover.PublishEvents(handler, ct);
        return handling.IsFailure ? removing.Error : toRemove;
    }
}
