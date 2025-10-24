using FluentValidation;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Entities.Tickets.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.ConfirmPasswordReset;

public sealed class ConfirmPasswordResetValidator : AbstractValidator<ConfirmPasswordResetCommand>
{
    public ConfirmPasswordResetValidator()
    {
        RuleFor(x => x.NewPassword).MustBeValid(UserPassword.Create);
        RuleFor(x => x.TicketId).MustBeValid(UserTicketId.Create);
    }
}
