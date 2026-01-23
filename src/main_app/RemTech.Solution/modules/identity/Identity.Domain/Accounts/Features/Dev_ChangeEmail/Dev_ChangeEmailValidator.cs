using FluentValidation;
using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace Identity.Domain.Accounts.Features.Dev_ChangeEmail;

public sealed class Dev_ChangeEmailValidator : AbstractValidator<Dev_ChangeEmailCommand>
{
    public Dev_ChangeEmailValidator() => RuleFor(x => x.NewEmail).MustBeValid(AccountEmail.Create);
}
