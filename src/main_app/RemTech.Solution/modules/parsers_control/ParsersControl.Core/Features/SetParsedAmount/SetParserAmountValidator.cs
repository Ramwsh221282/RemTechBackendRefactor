using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.SetParsedAmount;

/// <summary>
/// Валидатор команды установки количества распарсенных элементов.
/// </summary>
public sealed class SetParserAmountValidator : AbstractValidator<SetParsedAmountCommand>
{
	/// <summary>
	/// Инициализирует валидатор команды установки количества распарсенных элементов.
	/// </summary>
	public SetParserAmountValidator()
	{
		RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
	}
}
