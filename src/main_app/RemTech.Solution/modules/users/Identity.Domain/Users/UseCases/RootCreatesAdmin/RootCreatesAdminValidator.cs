using FluentValidation;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.RootCreatesAdmin;

public sealed class RootCreatesAdminValidator : AbstractValidator<RootCreatesAdminCommand>
{
    public RootCreatesAdminValidator()
    {
        RuleFor(x => x.CreatorId).MustBeValid(UserId.Create);
        RuleFor(x => x.CreatorPassword).MustBeValid(UserPassword.Create);
        RuleFor(x => x.NewUserLogin).MustBeValid(UserLogin.Create);
        RuleFor(x => x.NewUserEmail).MustBeValid(UserEmail.Create);
    }
}