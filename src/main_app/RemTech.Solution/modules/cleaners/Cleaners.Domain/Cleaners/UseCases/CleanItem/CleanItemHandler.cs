using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Aggregate.Decorators;
using Cleaners.Domain.Cleaners.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.UseCases.CleanItem;

public sealed class CleanItemHandler(ICleanersStorage cleaners, IDomainEventsDispatcher dispatcher)
    : ICommandHandler<CleanItemCommand, Status<Cleaner>>
{
    public async Task<Status<Cleaner>> Handle(
        CleanItemCommand command,
        CancellationToken ct = default
    )
    {
        var cleaner = await cleaners.Get(command.Id, ct);
        if (cleaner.IsFailure)
            return cleaner.Error;

        var eventual = new EventualCleaner(cleaner);
        var result = eventual.CleanItem();

        if (result.IsFailure)
            return result.Error;

        var handling = await eventual.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : cleaner;
    }
}
