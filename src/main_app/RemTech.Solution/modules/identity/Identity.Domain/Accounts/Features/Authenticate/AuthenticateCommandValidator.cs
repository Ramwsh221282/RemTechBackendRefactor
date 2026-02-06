using FluentValidation;
using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace Identity.Domain.Accounts.Features.Authenticate;

/// <summary>
/// Валидатор команды аутентификации пользователя.
/// </summary>
public sealed class AuthenticateCommandValidator : AbstractValidator<AuthenticateCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="AuthenticateCommandValidator"/>.
	/// </summary>
	public AuthenticateCommandValidator()
	{
		RuleFor(x => x.Email).MustBeValid(x => AccountEmail.Create(x!)).When(x => !string.IsNullOrWhiteSpace(x.Email));
		RuleFor(x => x.Login).MustBeValid(x => AccountLogin.Create(x!)).When(x => !string.IsNullOrWhiteSpace(x.Login));
		RuleFor(x => x.Password).NotEmpty().WithMessage("Пароль не указан.");
	}
}
