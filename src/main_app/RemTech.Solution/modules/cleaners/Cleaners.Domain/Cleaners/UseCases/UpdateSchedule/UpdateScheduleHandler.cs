using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Aggregate.Decorators;
using Cleaners.Domain.Cleaners.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.UseCases.UpdateSchedule;

public sealed class UpdateScheduleHandler(
    ICleanersStorage storage,
    IDomainEventsDispatcher dispatcher
) : ICommandHandler<UpdateScheduleCommand, Status<Cleaner>>
{
    public async Task<Status<Cleaner>> Handle(
        UpdateScheduleCommand command,
        CancellationToken ct = default
    )
    {
        var cleaner = await storage.Get(command.Id, ct);
        if (cleaner.IsFailure)
            return cleaner.Error;

        var eventual = new EventualCleaner(cleaner.Value);
        var result = eventual.UpdateSchedule(command.WaitDays);

        if (result.IsFailure)
            return result.Error;

        var handling = await eventual.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : cleaner;
    }
}
