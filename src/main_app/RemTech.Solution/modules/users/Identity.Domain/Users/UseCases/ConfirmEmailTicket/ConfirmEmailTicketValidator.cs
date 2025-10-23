using FluentValidation;
using Identity.Domain.Users.Entities.Tickets.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.ConfirmEmailTicket;

public sealed class ConfirmEmailTicketValidator : AbstractValidator<ConfirmEmailTicketCommand>
{
    public ConfirmEmailTicketValidator()
    {
        RuleFor(x => x.TicketId).MustBeValid(UserTicketId.Create);
    }
}
