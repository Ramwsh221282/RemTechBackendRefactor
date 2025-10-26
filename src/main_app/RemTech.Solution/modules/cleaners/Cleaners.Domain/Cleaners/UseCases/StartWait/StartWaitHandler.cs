using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Aggregate.Decorators;
using Cleaners.Domain.Cleaners.Ports;
using Cleaners.Domain.Cleaners.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.UseCases.StartWait;

public sealed class StartWaitHandler(ICleanersStorage cleaners, IDomainEventsDispatcher dispatcher)
    : ICommandHandler<StartWaitCommand, Status<Cleaner>>
{
    public async Task<Status<Cleaner>> Handle(
        StartWaitCommand command,
        CancellationToken ct = default
    )
    {
        var cleaner = await cleaners.Get(command.Id, ct);
        if (cleaner.IsFailure)
            return cleaner.Error;

        var eventual = new EventualCleaner(cleaner);
        var waiting = eventual.StartWait();

        if (waiting.IsFailure)
            return waiting.Error;

        var handling = await eventual.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : eventual;
    }
}
