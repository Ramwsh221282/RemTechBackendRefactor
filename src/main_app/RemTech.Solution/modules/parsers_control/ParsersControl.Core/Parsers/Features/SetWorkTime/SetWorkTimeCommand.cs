using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Parsers.Features.SetWorkTime;

public sealed record SetWorkTimeCommand(Guid Id, long TotalElapsedSeconds) : ICommand;

public sealed class SetWorkTimeValidator : AbstractValidator<SetWorkTimeCommand>
{
    public SetWorkTimeValidator()
    {
        RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
        RuleFor(x => x.TotalElapsedSeconds).GreaterThan(0);
    }
}