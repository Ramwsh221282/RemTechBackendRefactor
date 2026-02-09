using FluentValidation;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace Notifications.Core.Mailers.Features.DeleteMailer;

/// <summary>
/// Валидатор команды удаления почтового ящика.
/// </summary>
public sealed class DeleteMailerValidator : AbstractValidator<DeleteMailerCommand>
{
	/// <summary>
	///     Инициализирует новый экземпляр <see cref="DeleteMailerValidator"/>.
	/// </summary>
	public DeleteMailerValidator()
	{
		RuleFor(x => x.Id).MustBeValid(MailerId.Create);
	}
}
