using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.DisableParser;

public sealed class DisableParserValidator : AbstractValidator<DisableParserCommand>
{
    public DisableParserValidator()
    {
        RuleFor(c => c.Id).MustBeValid(SubscribedParserId.Create);
    }
}
