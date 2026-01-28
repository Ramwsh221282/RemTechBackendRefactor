using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.DisableParser;

/// <summary>
/// Валидатор команды отключения парсера.
/// </summary>
public sealed class DisableParserValidator : AbstractValidator<DisableParserCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="DisableParserValidator"/>.
	/// </summary>
	public DisableParserValidator()
	{
		RuleFor(c => c.Id).MustBeValid(SubscribedParserId.Create);
	}
}
