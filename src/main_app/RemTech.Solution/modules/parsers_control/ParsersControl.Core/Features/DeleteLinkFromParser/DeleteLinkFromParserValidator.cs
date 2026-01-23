using FluentValidation;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.DeleteLinkFromParser;

public sealed class DeleteLinkFromParserValidator : AbstractValidator<DeleteLinkFromParserCommand>
{
	public DeleteLinkFromParserValidator()
	{
		RuleFor(c => c.ParserId).MustBeValid(SubscribedParserId.Create);
		RuleFor(c => c.LinkId).MustBeValid(SubscribedParserLinkId.From);
	}
}
