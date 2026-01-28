using FluentValidation;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.SetLinkParsedAmount;

/// <summary>
/// Валидатор команды установки количества распарсенных ссылок.
/// </summary>
public sealed class SetLinkParsedAmountValidator : AbstractValidator<SetLinkParsedAmountCommand>
{
	/// <summary>
	///   Инициализирует новый экземпляр <see cref="SetLinkParsedAmountValidator"/>.
	/// </summary>
	public SetLinkParsedAmountValidator()
	{
		RuleFor(x => x.ParserId).MustBeValid(SubscribedParserId.Create);
		RuleFor(x => x.LinkId).MustBeValid(SubscribedParserLinkId.From);
	}
}
