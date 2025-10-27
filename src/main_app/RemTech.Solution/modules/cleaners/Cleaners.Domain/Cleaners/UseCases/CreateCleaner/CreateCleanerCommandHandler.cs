using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Aggregate.Decorators;
using Cleaners.Domain.Cleaners.Ports.Storage;
using Cleaners.Domain.Cleaners.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.UseCases.CreateCleaner;

public sealed class CreateCleanerCommandHandler(
    ICleanersStorage cleaners,
    IDomainEventsDispatcher dispatcher
) : ICommandHandler<CreateCleanerCommand, Status<Cleaner>>
{
    public async Task<Status<Cleaner>> Handle(
        CreateCleanerCommand command,
        CancellationToken ct = default
    )
    {
        var cleaner = await cleaners.Get(ct);
        if (cleaner.IsSuccess)
            return Error.Conflict("Чистильщик уже создан.");

        CleanerSchedule schedule = new();
        CleanerWorkTime workTime = new();
        int cleanedAmount = 0;
        string state = Cleaner.DisabledState;
        int itemsThreshold = 5;

        var created = EventualCleaner.Create(
            schedule,
            workTime,
            cleanedAmount,
            state,
            itemsThreshold
        );

        if (created.IsFailure)
            return created.Error;

        var handling = await created.Value.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : created.Value;
    }
}
