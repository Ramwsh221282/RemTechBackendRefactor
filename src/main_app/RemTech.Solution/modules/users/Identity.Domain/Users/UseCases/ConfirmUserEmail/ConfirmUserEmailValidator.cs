using FluentValidation;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.ConfirmUserEmail;

public sealed class ConfirmUserEmailValidator : AbstractValidator<ConfirmUserEmailCommand>
{
    public ConfirmUserEmailValidator()
    {
        RuleFor(x => x.TicketId).MustBeValid(IdentityTokenId.Create);
    }
}