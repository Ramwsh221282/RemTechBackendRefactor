using FluentValidation;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.ChangeLinkActivity;

/// <summary>
/// Валидатор команды изменения активности ссылки на парсер.
/// </summary>
public sealed class ChangeLinkActivityLinkValidator : AbstractValidator<ChangeLinkActivityCommand>
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="ChangeLinkActivityLinkValidator"/>.
	/// </summary>
	public ChangeLinkActivityLinkValidator()
	{
		RuleFor(x => x.ParserId).MustBeValid(SubscribedParserId.Create);
		RuleFor(x => x.LinkId).MustBeValid(SubscribedParserLinkId.From);
	}
}
