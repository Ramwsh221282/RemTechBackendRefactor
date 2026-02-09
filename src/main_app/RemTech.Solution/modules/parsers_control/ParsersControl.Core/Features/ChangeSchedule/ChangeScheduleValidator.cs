using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.ChangeSchedule;

/// <summary>
/// Валидатор команды изменения расписания парсера.
/// </summary>
public sealed class ChangeScheduleValidator : AbstractValidator<ChangeScheduleCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="ChangeScheduleValidator"/>.
	/// </summary>
	public ChangeScheduleValidator()
	{
		RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
	}
}
