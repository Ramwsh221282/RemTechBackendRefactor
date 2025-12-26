using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Aggregate.Decorators;
using Cleaners.Domain.Cleaners.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.UseCases.AdjustWorkTime;

public sealed class AdjustWorkTimeHandler(
    ICleanersStorage cleaners,
    IDomainEventsDispatcher dispatcher
) : ICommandHandler<AdjustWorkTimeCommand, Status<Cleaner>>
{
    public async Task<Status<Cleaner>> Handle(
        AdjustWorkTimeCommand command,
        CancellationToken ct = default
    )
    {
        var cleaner = await cleaners.Get(command.Id, ct);
        if (cleaner.IsFailure)
            return cleaner.Error;

        var eventual = new EventualCleaner(cleaner);
        var adjusting = eventual.AdjustWorkTimeByTotalSeconds(command.TotalSeconds);

        if (adjusting.IsFailure)
            return adjusting.Error;

        var handling = await eventual.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : cleaner;
    }
}
