using FluentValidation;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace ParsersControl.Core.Features.ChangeSchedule;

public sealed class ChangeScheduleValidator : AbstractValidator<ChangeScheduleCommand>
{
    public ChangeScheduleValidator()
    {
        RuleFor(x => x.Id).MustBeValid(SubscribedParserId.Create);
    }
}
