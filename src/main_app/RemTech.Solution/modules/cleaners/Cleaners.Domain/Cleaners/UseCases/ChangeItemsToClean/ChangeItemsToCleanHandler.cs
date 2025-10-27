using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Aggregate.Decorators;
using Cleaners.Domain.Cleaners.Ports;
using Cleaners.Domain.Cleaners.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.UseCases.ChangeItemsToClean;

public sealed class ChangeItemsToCleanHandler(
    ICleanersStorage cleaners,
    IDomainEventsDispatcher dispatcher
) : ICommandHandler<ChangeItemsToCleanTreshold, Status<Cleaner>>
{
    public async Task<Status<Cleaner>> Handle(
        ChangeItemsToCleanTreshold command,
        CancellationToken ct = default
    )
    {
        var cleaner = await cleaners.Get(command.Id, ct);
        if (cleaner.IsFailure)
            return cleaner.Error;

        var eventual = new EventualCleaner(cleaner);
        var result = eventual.ChangeItemsToCleanThreshold(command.Threshold);

        if (result.IsFailure)
            return result.Error;

        var handling = await eventual.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : cleaner;
    }
}
