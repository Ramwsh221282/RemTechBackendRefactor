using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Parsers.Features.SetWorkTime;

public sealed class SetWorkTimeValidator : AbstractValidator<SetWorkTimeCommand>
{
    public SetWorkTimeValidator()
    {
        RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
        RuleFor(x => x.TotalElapsedSeconds).MustBeValid(SubscribedParserWorkTimeStatistics.FromTotalElapsedSeconds);
    }
}