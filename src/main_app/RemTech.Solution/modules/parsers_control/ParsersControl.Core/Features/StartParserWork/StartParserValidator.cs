using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.StartParserWork;

public sealed class StartParserValidator : AbstractValidator<StartParserCommand>
{
    public StartParserValidator()
    {
        RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
    }
}
