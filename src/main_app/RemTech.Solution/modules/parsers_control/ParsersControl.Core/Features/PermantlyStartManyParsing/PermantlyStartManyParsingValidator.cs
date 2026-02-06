using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.PermantlyStartManyParsing;

/// <summary>
/// Валидатор команды постоянного запуска множества парсеров.
/// </summary>
public sealed class PermantlyStartManyParsingValidator : AbstractValidator<PermantlyStartManyParsingCommand>
{
	/// <summary>
	///    Инициализирует новый экземпляр <see cref="PermantlyStartManyParsingValidator"/>.
	/// </summary>
	public PermantlyStartManyParsingValidator()
	{
		RuleFor(x => x.Identifiers).EachMustFollow([SubscribedParserId.Create]);
	}
}
