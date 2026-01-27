using FluentValidation;
using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace Identity.Domain.Accounts.Features.Dev_ChangeEmail;

/// <summary>
/// Валидатор команды изменения email пользователя в режиме разработки.
/// </summary>
public sealed class Dev_ChangeEmailValidator : AbstractValidator<Dev_ChangeEmailCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="Dev_ChangeEmailValidator"/>.
	/// </summary>
	public Dev_ChangeEmailValidator()
	{
		RuleFor(x => x.NewEmail).MustBeValid(AccountEmail.Create);
	}
}
