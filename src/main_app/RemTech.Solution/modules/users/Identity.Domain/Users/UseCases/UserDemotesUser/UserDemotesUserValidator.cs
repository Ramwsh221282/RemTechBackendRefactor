using FluentValidation;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.UserDemotesUser;

public sealed class UserDemotesUserValidator : AbstractValidator<UserDemotesUserCommand>
{
    public UserDemotesUserValidator()
    {
        RuleFor(x => x.DemoterId).MustBeValid(UserId.Create);
        RuleFor(x => x.DemoterPassword).MustBeValid(UserPassword.Create);
        RuleFor(x => x.UserId).MustBeValid(UserId.Create);
        RuleFor(x => x.RoleName).MustBeValid(UserLogin.Create);
    }
}
