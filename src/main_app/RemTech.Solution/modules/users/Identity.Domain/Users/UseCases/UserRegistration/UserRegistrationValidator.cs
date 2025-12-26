using FluentValidation;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.UserRegistration;

public sealed class UserRegistrationValidator : AbstractValidator<UserRegistrationCommand>
{
    public UserRegistrationValidator()
    {
        RuleFor(x => x.UserLogin).MustBeValid(UserLogin.Create);
        RuleFor(x => x.UserEmail).MustBeValid(UserEmail.Create);
        RuleFor(x => x.UserPassword).MustBeValid(UserPassword.Create);
    }
}
