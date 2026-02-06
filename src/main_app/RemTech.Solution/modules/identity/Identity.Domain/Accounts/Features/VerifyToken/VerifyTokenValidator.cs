using FluentValidation;

namespace Identity.Domain.Accounts.Features.VerifyToken;

/// <summary>
/// Валидатор команды для проверки токена пользователя.
/// </summary>
public sealed class VerifyTokenValidator : AbstractValidator<VerifyTokenCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="VerifyTokenValidator"/>.
	/// </summary>
	public VerifyTokenValidator()
	{
		RuleFor(x => x.Token).NotEmpty().WithMessage("Токен не предоставлен.");
	}
}
