using FluentValidation;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace Notifications.Core.Mailers.Features.AddMailer;

/// <summary>
/// Валидатор команды добавления почтового ящика.
/// </summary>
public sealed class AddMailerCommandValidator : AbstractValidator<AddMailerCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="AddMailerCommandValidator"/>.
	/// </summary>
	public AddMailerCommandValidator()
	{
		RuleFor(c => new { c.Email, c.SmtpPassword })
			.MustBeValid(o => MailerCredentials.Create(o.SmtpPassword, o.Email));
	}
}
