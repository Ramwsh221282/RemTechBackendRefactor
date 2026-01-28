using FluentValidation;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace Notifications.Core.PendingEmails.Features.AddPendingEmail;

/// <summary>
/// Валидатор команды для добавления нового ожидающего email-уведомления.
/// </summary>
public sealed class AddPendingEmailValidator : AbstractValidator<AddPendingEmailCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="AddPendingEmailValidator"/>.
	/// </summary>
	public AddPendingEmailValidator()
	{
		RuleFor(x => new
			{
				x.Recipient,
				x.Subject,
				x.Body,
			})
			.MustBeValid(o => PendingEmailNotification.CreateNew(o.Recipient, o.Subject, o.Body));
	}
}
