using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.PermantlyDisableParsing;

/// <summary>
/// Валидатор команды постоянного отключения парсера.
/// </summary>
public sealed class PermantlyDisableParsingValidator : AbstractValidator<PermantlyDisableParsingCommand>
{
	/// <summary>
	///     Инициализирует новый экземпляр <see cref="PermantlyDisableParsingValidator"/>.
	/// </summary>
	public PermantlyDisableParsingValidator()
	{
		RuleFor(c => c.Id).MustBeValid(SubscribedParserId.Create);
	}
}
