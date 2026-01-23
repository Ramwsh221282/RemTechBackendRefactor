using FluentValidation;
using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace Identity.Domain.Accounts.Features.Dev_ChangePassword;

public sealed class Dev_ChangePasswordValidator : AbstractValidator<Dev_ChangePasswordCommand>
{
	public Dev_ChangePasswordValidator() => RuleFor(x => x.NewPassword).MustBeValid(AccountPassword.Create);
}
