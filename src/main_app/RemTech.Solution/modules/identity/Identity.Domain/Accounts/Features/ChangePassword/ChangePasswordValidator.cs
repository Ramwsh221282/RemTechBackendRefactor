using FluentValidation;
using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.ChangePassword;

public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.NewPassword).MustBeValid(AccountPassword.Create);
        RuleFor(x => x.CurrentPassword).MustBeValid(AccountPassword.Create);
    }
}