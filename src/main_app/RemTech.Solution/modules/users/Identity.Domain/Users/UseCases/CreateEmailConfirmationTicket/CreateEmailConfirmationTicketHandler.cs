using FluentValidation;
using FluentValidation.Results;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.UseCases.Common;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.CreateEmailConfirmationTicket;

public sealed class CreateEmailConfirmationTicketHandler(
    IGetVerifiedUserHandle getVerifiedUser,
    IDomainEventsDispatcher dispatcher,
    IValidator<CreateEmailConfirmationTicketCommand> validator
) : ICommandHandler<CreateEmailConfirmationTicketCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        CreateEmailConfirmationTicketCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await validator.ValidateAsync(command, ct);
        if (validation.IsValid == false)
            return validation.ValidationError();

        Status<User> user = await getVerifiedUser.Handle(command.UserId, command.Password, ct);
        if (user.IsFailure)
            return user.Error;

        Status ticketCreation = user.Value.FormConfirmationTicket();
        if (ticketCreation.IsFailure)
            return ticketCreation.Error;

        Status handling = await user.Value.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : user;
    }
}
