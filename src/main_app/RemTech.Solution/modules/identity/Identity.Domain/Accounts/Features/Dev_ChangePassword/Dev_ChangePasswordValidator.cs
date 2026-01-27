using FluentValidation;
using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace Identity.Domain.Accounts.Features.Dev_ChangePassword;

/// <summary>
/// Валидатор команды изменения пароля пользователя в режиме разработки.
/// </summary>
public sealed class Dev_ChangePasswordValidator : AbstractValidator<Dev_ChangePasswordCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="Dev_ChangePasswordValidator"/>.
	/// </summary>
	public Dev_ChangePasswordValidator() => RuleFor(x => x.NewPassword).MustBeValid(AccountPassword.Create);
}
