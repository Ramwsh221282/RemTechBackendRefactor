using FluentValidation;
using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace Identity.Domain.Accounts.Features.ChangePassword;

/// <summary>
/// Валидатор команды изменения пароля пользователя.
/// </summary>
public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="ChangePasswordValidator"/>.
	/// </summary>
	public ChangePasswordValidator()
	{
		RuleFor(x => x.NewPassword).MustBeValid(AccountPassword.Create);
		RuleFor(x => x.CurrentPassword).MustBeValid(AccountPassword.Create);
	}
}
