using FluentValidation;

namespace Identity.Domain.Accounts.Features.Refresh;

/// <summary>
/// Валидатор команды для обновления токенов пользователя.
/// </summary>
public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="RefreshTokenValidator"/>.
	/// </summary>
	public RefreshTokenValidator()
	{
		RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Token not provided.");
		RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh token not provided.");
	}
}
