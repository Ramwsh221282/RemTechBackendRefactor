using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.StopParserWork;

public sealed class StopParserWorkValidator : AbstractValidator<StopParserWorkCommand>
{
    public StopParserWorkValidator()
    {
        RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
    }
}
