using FluentValidation;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.AdminCreatesUser;

public sealed class AdminCreatesUserValidator : AbstractValidator<AdminCreatesUserCommand>
{
    public AdminCreatesUserValidator()
    {
        RuleFor(x => x.CreatorId).MustBeValid(UserId.Create);
        RuleFor(x => x.CreatorPassword).MustBeValid(UserPassword.Create);
        RuleFor(x => x.NewUserEmail).MustBeValid(UserEmail.Create);
        RuleFor(x => x.NewUserLogin).MustBeValid(UserLogin.Create);
    }
}
