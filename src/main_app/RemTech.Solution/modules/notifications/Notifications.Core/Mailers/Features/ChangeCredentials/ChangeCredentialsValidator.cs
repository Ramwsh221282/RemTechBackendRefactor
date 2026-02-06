using FluentValidation;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace Notifications.Core.Mailers.Features.ChangeCredentials;

/// <summary>
/// Валидатор команды изменения учетных данных почтового ящика.
/// </summary>
public sealed class ChangeCredentialsValidator : AbstractValidator<ChangeCredentialsCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="ChangeCredentialsValidator"/>.
	/// </summary>
	public ChangeCredentialsValidator()
	{
		RuleFor(c => c.Id).MustBeValid(MailerId.Create);
		RuleFor(c => new { c.SmtpPassword, c.Email })
			.MustBeValid(o => MailerCredentials.Create(o.SmtpPassword, o.Email));
	}
}
