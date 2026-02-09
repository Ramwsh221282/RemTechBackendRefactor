using FluentValidation;
using ParsersControl.Core.Features.PermantlyStartManyParsing;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.PermantlyDisableManyParsing;

/// <summary>
/// Валидатор команды постоянного отключения множества парсеров.
/// </summary>
public sealed class PermantlyDisableManyParsingValidator : AbstractValidator<PermantlyStartManyParsingCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="PermantlyDisableManyParsingValidator"/>.
	/// </summary>
	public PermantlyDisableManyParsingValidator()
	{
		RuleFor(x => x.Identifiers).EachMustFollow([SubscribedParserId.Create]);
	}
}
