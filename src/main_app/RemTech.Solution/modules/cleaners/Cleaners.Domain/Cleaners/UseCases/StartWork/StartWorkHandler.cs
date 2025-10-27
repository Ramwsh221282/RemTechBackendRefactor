using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Aggregate.Decorators;
using Cleaners.Domain.Cleaners.Ports;
using Cleaners.Domain.Cleaners.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.UseCases.StartWork;

public sealed class StartWorkHandler(ICleanersStorage cleaners, IDomainEventsDispatcher dispatcher)
    : ICommandHandler<StartWorkCommand, Status<Cleaner>>
{
    public async Task<Status<Cleaner>> Handle(
        StartWorkCommand command,
        CancellationToken ct = default
    )
    {
        var cleaner = await cleaners.Get(command.Id, ct);
        if (cleaner.IsFailure)
            return cleaner.Error;

        var eventual = new EventualCleaner(cleaner.Value);
        var starting = eventual.StartWork();

        if (starting.IsFailure)
            return starting.Error;

        var handling = await eventual.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : cleaner;
    }
}
