using Cleaners.Domain.Cleaners.Events;
using Cleaners.Domain.Cleaners.ValueObjects;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.Aggregate.Decorators;

public sealed class EventualCleaner : Cleaner
{
    private readonly List<IDomainEvent> _events;

    private readonly Cleaner _origin;

    public EventualCleaner(Cleaner cleaner)
        : base(cleaner)
    {
        _origin = cleaner;
        _events = [];
    }

    public static Status<EventualCleaner> Create(
        CleanerSchedule schedule,
        CleanerWorkTime workTime,
        int cleanedAmount,
        string state,
        int itemsDateDayThreshold,
        Guid? id = null
    )
    {
        var cleaner = LogicalCleaner.Create(
            schedule,
            workTime,
            cleanedAmount,
            state,
            itemsDateDayThreshold,
            id
        );
        if (cleaner.IsFailure)
            return cleaner.Error;

        EventualCleaner eventual = new(cleaner);

        if (id == null)
            eventual._events.Add(new CleanerCreatedEvent(eventual));

        return eventual;
    }

    public override Status UpdateSchedule(int waitDays)
    {
        Status result = _origin.UpdateSchedule(waitDays);
        _events.Add(new CleanerScheduleUpdated(_origin));
        return result;
    }

    public override Status StartWork()
    {
        Status result = _origin.StartWork();
        _events.Add(new CleanerWorkStarted(_origin));
        return result;
    }

    public override Status StartWait()
    {
        Status result = _origin.StartWait();
        _events.Add(new CleanerStartedWaiting(_origin));
        return result;
    }

    public override Status Disable()
    {
        Status result = _origin.Disable();
        _events.Add(new CleanerDisabled(_origin));
        return result;
    }

    public override Status AdjustWorkTimeByTotalSeconds(long totalSeconds)
    {
        Status result = _origin.AdjustWorkTimeByTotalSeconds(totalSeconds);
        _events.Add(new CleanerWorkTimeChanged(_origin));
        return result;
    }

    public override Status ChangeItemsToCleanThreshold(int threshold)
    {
        Status result = _origin.ChangeItemsToCleanThreshold(threshold);
        _events.Add(new CleanerItemsDateDayThresholdChanged(_origin));
        return result;
    }

    public override Status CleanItem()
    {
        Status result = _origin.CleanItem();
        _events.Add(new CleanerCleanedItem(_origin));
        return result;
    }

    public async Task<Status> PublishEvents(
        IDomainEventsDispatcher dispatcher,
        CancellationToken ct = default
    ) => await dispatcher.Dispatch(_events, ct);
}
