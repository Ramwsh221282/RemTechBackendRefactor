using FluentValidation;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.DeleteLinkFromParser;

/// <summary>
/// Валидатор команды удаления ссылки из парсера.
/// </summary>
public sealed class DeleteLinkFromParserValidator : AbstractValidator<DeleteLinkFromParserCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="DeleteLinkFromParserValidator"/>.
	/// </summary>
	public DeleteLinkFromParserValidator()
	{
		RuleFor(c => c.ParserId).MustBeValid(SubscribedParserId.Create);
		RuleFor(c => c.LinkId).MustBeValid(SubscribedParserLinkId.From);
	}
}
