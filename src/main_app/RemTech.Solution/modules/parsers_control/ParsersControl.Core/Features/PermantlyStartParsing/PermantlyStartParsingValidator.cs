using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.PermantlyStartParsing;

public sealed class PermantlyStartParsingValidator : AbstractValidator<PermantlyStartParsingCommand>
{
	public PermantlyStartParsingValidator() => RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
}
