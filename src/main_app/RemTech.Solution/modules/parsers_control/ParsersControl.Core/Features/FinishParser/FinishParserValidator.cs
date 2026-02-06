using FluentValidation;
using ParsersControl.Core.Common;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.FinishParser;

/// <summary>
/// Валидатор команды завершения парсера.
/// </summary>
public sealed class FinishParserValidator : AbstractValidator<FinishParserCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="FinishParserValidator"/>.
	/// </summary>
	public FinishParserValidator()
	{
		RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
		RuleFor(x => x.TotalElapsedSeconds).MustBeValid(ParsingWorkTime.FromTotalElapsedSeconds);
	}
}
