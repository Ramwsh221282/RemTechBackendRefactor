using FluentValidation;
using ParsersControl.Core.Common;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.SetWorkTime;

/// <summary>
/// Валидатор команды установки рабочего времени.
/// </summary>
public sealed class SetWorkTimeValidator : AbstractValidator<SetWorkTimeCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="SetWorkTimeValidator"/>.
	/// </summary>
	public SetWorkTimeValidator()
	{
		RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
		RuleFor(x => x.TotalElapsedSeconds).MustBeValid(ParsingWorkTime.FromTotalElapsedSeconds);
	}
}
