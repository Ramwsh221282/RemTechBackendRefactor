using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.EnableParser;

public sealed class EnableParserValidator : AbstractValidator<EnableParserCommand>
{
    public EnableParserValidator()
    {
        RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
    }
}
