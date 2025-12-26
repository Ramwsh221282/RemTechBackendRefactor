using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Aggregate.Decorators;
using Cleaners.Domain.Cleaners.Ports.Storage;
using Cleaners.Domain.Cleaners.ValueObjects;
using FluentValidation;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Cleaners.Domain.Cleaners.UseCases.UpdateWorkStatistics;

public sealed class UpdateWorkStatisticsHandler(
    ICleanersStorage storage,
    IValidator<UpdateWorkStatisticsCommand> validator,
    IDomainEventsDispatcher dispatcher
) : ICommandHandler<UpdateWorkStatisticsCommand, Status<Cleaner>>
{
    public async Task<Status<Cleaner>> Handle(
        UpdateWorkStatisticsCommand command,
        CancellationToken ct = default
    )
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.ValidationError();

        var cleaner = await storage.Get(command.Id, ct);
        if (cleaner.IsFailure)
            return cleaner.Error;

        var eventual = new EventualCleaner(cleaner);
        var workTime = CleanerWorkTime.Create(command.ElapsedSeconds);
        var updating = eventual.UpdateWorkStatistics(workTime, (int)command.ProcessedItems);

        if (updating.IsFailure)
            return updating.Error;

        var handling = await eventual.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : cleaner;
    }
}
