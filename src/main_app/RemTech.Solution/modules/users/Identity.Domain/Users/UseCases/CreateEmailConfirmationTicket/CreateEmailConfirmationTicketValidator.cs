using FluentValidation;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.CreateEmailConfirmationTicket;

public sealed class CreateEmailConfirmationTicketValidator
    : AbstractValidator<CreateEmailConfirmationTicketCommand>
{
    public CreateEmailConfirmationTicketValidator()
    {
        RuleFor(x => x.UserId).MustBeValid(UserId.Create);
        RuleFor(x => x.Password).MustBeValid(UserPassword.Create);
    }
}
