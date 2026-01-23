using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.PermantlyDisableParsing;

public sealed class PermantlyDisableParsingValidator : AbstractValidator<PermantlyDisableParsingCommand>
{
    public PermantlyDisableParsingValidator()
    {
        RuleFor(c => c.Id).MustBeValid(SubscribedParserId.Create);
    }
}
