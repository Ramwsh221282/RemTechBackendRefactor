using Cleaners.Domain.Cleaners.ValueObjects;
using FluentValidation;
using RemTech.Core.Shared.Validation;

namespace Cleaners.Domain.Cleaners.UseCases.UpdateWorkStatistics;

public sealed class UpdateWorkStatisticsValidator : AbstractValidator<UpdateWorkStatisticsCommand>
{
    public UpdateWorkStatisticsValidator()
    {
        RuleFor(x => x.ElapsedSeconds).MustBeValid(x => CleanerWorkTime.Create(x));
        RuleFor(x => x.ProcessedItems).GreaterThan(-1);
    }
}
