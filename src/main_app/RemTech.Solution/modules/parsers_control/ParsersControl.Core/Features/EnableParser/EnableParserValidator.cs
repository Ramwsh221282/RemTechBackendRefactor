using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.EnableParser;

/// <summary>
/// Валидатор команды включения парсера.
/// </summary>
public sealed class EnableParserValidator : AbstractValidator<EnableParserCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="EnableParserValidator"/>.
	/// </summary>
	public EnableParserValidator()
	{
		RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
	}
}
