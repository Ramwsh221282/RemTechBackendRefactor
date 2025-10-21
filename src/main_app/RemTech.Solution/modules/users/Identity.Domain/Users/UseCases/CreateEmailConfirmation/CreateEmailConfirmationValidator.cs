using FluentValidation;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Users.UseCases.CreateEmailConfirmation;

public sealed class CreateEmailConfirmationValidator
    : AbstractValidator<CreateEmailConfirmationCommand>
{
    public CreateEmailConfirmationValidator()
    {
        RuleFor(x => x.UserId).MustBeValid(UserId.Create);
    }
}