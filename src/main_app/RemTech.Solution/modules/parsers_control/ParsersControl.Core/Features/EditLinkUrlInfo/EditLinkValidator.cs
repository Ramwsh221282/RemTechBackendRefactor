using FluentValidation;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.EditLinkUrlInfo;

/// <summary>
/// Валидатор команды редактирования информации о ссылке и URL парсера.
/// </summary>
public sealed class EditLinkValidator : AbstractValidator<EditLinkUrlInfoCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="EditLinkValidator"/>.
	/// </summary>
	public EditLinkValidator()
	{
		RuleFor(x => x.ParserId).MustBeValid(SubscribedParserId.Create);
		RuleFor(x => x.LinkId).MustBeValid(SubscribedParserLinkId.From);
	}
}
