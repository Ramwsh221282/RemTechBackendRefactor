using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.SubscribeParser;

public sealed class SubscribeParserValidator : AbstractValidator<SubscribeParserCommand>
{
	public SubscribeParserValidator()
	{
		RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
		RuleFor(x => new { x.Domain, x.Type }).MustBeValid(x => SubscribedParserIdentity.Create(x.Domain, x.Type));
	}
}
