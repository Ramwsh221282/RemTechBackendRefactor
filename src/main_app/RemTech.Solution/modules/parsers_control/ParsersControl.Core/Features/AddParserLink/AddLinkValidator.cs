using FluentValidation;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.AddParserLink;

public sealed class AddLinkValidator : AbstractValidator<AddParserLinkCommand>
{
	public AddLinkValidator()
	{
		RuleFor(x => x.ParserId).MustBeValid(SubscribedParserId.Create);
		RuleFor(x => x.Links).AllMustBeValid(x => SubscribedParserLinkUrlInfo.Create(x.LinkUrl, x.LinkName));
	}
}
