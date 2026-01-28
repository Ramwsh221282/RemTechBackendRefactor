using FluentValidation;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.SetLinkWorkTime;

/// <summary>
/// Валидатор команды установки рабочего времени ссылки.
/// </summary>
public sealed class SetLinkWorkTimeValidator : AbstractValidator<SetLinkWorkingTimeCommand>
{
	/// <summary>
	///  Инициализирует новый экземпляр <see cref="SetLinkWorkTimeValidator"/>.
	/// </summary>
	public SetLinkWorkTimeValidator()
	{
		RuleFor(x => x.ParserId).MustBeValid(SubscribedParserId.Create);
		RuleFor(x => x.LinkId).MustBeValid(SubscribedParserLinkId.From);
	}
}
