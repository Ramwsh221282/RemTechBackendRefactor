using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.StopParserWork;

/// <summary>
/// Валидатор команды остановки работы парсера.
/// </summary>
public sealed class StopParserWorkValidator : AbstractValidator<StopParserWorkCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="StopParserWorkValidator"/>.
	/// </summary>
	public StopParserWorkValidator()
	{
		RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
	}
}
