using FluentValidation;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.UserPromotesUser;

public sealed class UserPromotesUserValidator : AbstractValidator<UserPromotesUserCommand>
{
    public UserPromotesUserValidator()
    {
        RuleFor(x => x.PromoterId).MustBeValid(UserId.Create);
        RuleFor(x => x.PromoterPassword).MustBeValid(UserPassword.Create);
        RuleFor(x => x.UserId).MustBeValid(UserId.Create);
        RuleFor(x => x.RoleName).MustBeValid(UserLogin.Create);
    }
}
