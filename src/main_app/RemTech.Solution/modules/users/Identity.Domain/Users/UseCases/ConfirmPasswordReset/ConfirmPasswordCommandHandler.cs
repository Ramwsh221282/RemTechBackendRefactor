using FluentValidation;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Entities.Tickets.ValueObjects;
using Identity.Domain.Users.Ports.Passwords;
using Identity.Domain.Users.UseCases.Common;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.ConfirmPasswordReset;

public sealed class ConfirmPasswordCommandHandler(
    IGetUserByTicketHandle getUser,
    IStringHashAlgorithm hash,
    IValidator<ConfirmPasswordResetCommand> validator,
    IDomainEventsDispatcher dispatcher
) : ICommandHandler<ConfirmPasswordResetCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        ConfirmPasswordResetCommand command,
        CancellationToken ct = default
    )
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (validation.IsValid == false)
            return validation.ValidationError();

        var user = await getUser.Handle(command.TicketId, ct);
        if (user.IsFailure)
            return user.Error;

        var id = UserTicketId.Create(command.TicketId);
        var password = UserPassword.Create(command.NewPassword);
        var hashed = password.Value.Hash(hash);
        var confirmation = user.Value.ResetPassword(id, hashed);

        if (confirmation.IsFailure)
            return confirmation.Error;

        var handling = await user.Value.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : user;
    }
}
