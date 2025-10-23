using FluentValidation;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities.Tickets.ValueObjects;
using Identity.Domain.Users.UseCases.Common;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.ConfirmEmailTicket;

public sealed class ConfirmEmailTicketHandler(
    IGetUserByTicketHandle getUser,
    IDomainEventsDispatcher dispatcher,
    IValidator<ConfirmEmailTicketCommand> validator
) : ICommandHandler<ConfirmEmailTicketCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        ConfirmEmailTicketCommand command,
        CancellationToken ct = default
    )
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (validation.IsValid == false)
            return validation.ValidationError();

        UserTicketId id = UserTicketId.Create(command.TicketId);
        var user = await getUser.Handle(id.Id, ct);
        if (user.IsFailure)
            return user.Error;

        Status confirmation = user.Value.ConfirmEmail(id);
        if (confirmation.IsFailure)
            return confirmation.Error;

        Status handling = await user.Value.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : user;
    }
}
