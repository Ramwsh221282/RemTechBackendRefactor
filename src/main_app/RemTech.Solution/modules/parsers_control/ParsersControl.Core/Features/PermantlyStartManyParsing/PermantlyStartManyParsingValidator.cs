using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.PermantlyStartManyParsing;

public sealed class PermantlyStartManyParsingValidator : AbstractValidator<PermantlyStartManyParsingCommand>
{
	public PermantlyStartManyParsingValidator()
	{
		RuleFor(x => x.Identifiers).EachMustFollow([SubscribedParserId.Create]);
	}
}
