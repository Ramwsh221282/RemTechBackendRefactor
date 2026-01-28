using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.StartParserWork;

/// <summary>
/// Валидатор команды начала работы парсера.
/// </summary>
public sealed class StartParserValidator : AbstractValidator<StartParserCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="StartParserValidator"/>.
	/// </summary>
	public StartParserValidator()
	{
		RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
	}
}
