using FluentValidation;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.ChangingEmail;

public sealed class ChangeEmailValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.ChangerId).MustBeValid(UserId.Create);
        RuleFor(x => x.NewEmail).MustBeValid(UserEmail.Create);
    }
}
