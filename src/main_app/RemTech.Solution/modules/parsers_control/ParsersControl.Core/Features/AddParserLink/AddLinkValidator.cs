using FluentValidation;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.AddParserLink;

/// <summary>
/// Валидатор команды добавления ссылки на парсер.
/// </summary>
public sealed class AddLinkValidator : AbstractValidator<AddParserLinkCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="AddLinkValidator"/>.
	/// </summary>
	public AddLinkValidator()
	{
		RuleFor(x => x.ParserId).MustBeValid(SubscribedParserId.Create);
		RuleFor(x => x.Links).AllMustBeValid(x => SubscribedParserLinkUrlInfo.Create(x.LinkUrl, x.LinkName));
	}
}
