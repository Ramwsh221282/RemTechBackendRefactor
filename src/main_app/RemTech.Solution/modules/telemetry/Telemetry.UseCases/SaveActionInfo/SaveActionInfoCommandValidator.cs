using FluentValidation;
using RemTech.UseCases.Shared.Validations;
using Telemetry.Domain.TelemetryContext.ValueObjects;

namespace Telemetry.UseCases.SaveActionInfo;

/// <summary>
/// Валидатор команды добавления действия
/// </summary>
public sealed class SaveActionInfoCommandValidator : AbstractValidator<SaveActionInfoIbCommand>
{
    public SaveActionInfoCommandValidator()
    {
        RuleFor(x => x.Comments).AllMustBeValid(TelemetryActionComment.Create);
        RuleFor(x => x.InvokerId).MustBeValid(TelemetryInvokerId.Create);
        RuleFor(x => x.Name).MustBeValid(TelemetryActionName.Create);
        RuleFor(x => x.OccuredAt).MustBeValid(TelemetryRecordDate.Create);
        RuleFor(x => x.Status).MustBeValid(TelemetryActionStatus.Create);
    }
}
