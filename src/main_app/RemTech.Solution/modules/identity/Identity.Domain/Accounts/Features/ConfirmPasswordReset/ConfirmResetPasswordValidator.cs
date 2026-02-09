using FluentValidation;
using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace Identity.Domain.Accounts.Features.ConfirmPasswordReset;

/// <summary>
/// Валидатор команды подтверждения сброса пароля пользователя.
/// </summary>
public sealed class ConfirmResetPasswordValidator : AbstractValidator<ConfirmResetPasswordCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="ConfirmResetPasswordValidator"/>.
	/// </summary>
	public ConfirmResetPasswordValidator()
	{
		RuleFor(x => x.AccountId).MustBeValid(AccountId.Create);
		RuleFor(x => x.TicketId).MustBeValid(AccountId.Create);
		RuleFor(x => x.NewPassword).MustBeValid(AccountPassword.Create);
	}
}
