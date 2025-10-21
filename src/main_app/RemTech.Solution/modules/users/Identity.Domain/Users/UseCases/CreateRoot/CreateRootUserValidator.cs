using FluentValidation;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.CreateRoot;

public sealed class CreateRootUserValidator : AbstractValidator<CreateRootUserCommand>
{
    public CreateRootUserValidator()
    {
        RuleFor(x => x.Email).MustBeValid(UserEmail.Create);
        RuleFor(x => x.Name).MustBeValid(UserLogin.Create);
        RuleFor(x => x.Password).MustBeValid(UserPassword.Create);
    }
}