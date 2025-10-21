using FluentValidation;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.UserRemovesUser;

public sealed class UserRemovesUserValidator : AbstractValidator<UserRemovesUserCommand>
{
    public UserRemovesUserValidator()
    {
        RuleFor(x => x.RemoverId).MustBeValid(UserId.Create);
        RuleFor(x => x.RemovalId).MustBeValid(UserId.Create);
    }
}