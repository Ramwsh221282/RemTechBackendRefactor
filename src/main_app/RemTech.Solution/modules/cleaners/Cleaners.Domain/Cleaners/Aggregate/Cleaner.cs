using Cleaners.Domain.Cleaners.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.Aggregate;

public abstract class Cleaner
{
    protected const string WorkState = "Работает";
    protected const string WaitingState = "Ожидает";
    protected const string DisabledState = "Отключен";
    protected static readonly string[] AllowedStates = ["Работает", "Ожидает", "Отключен"];
    public Guid Id { get; }
    public CleanerSchedule Schedule { get; protected set; }
    public CleanerWorkTime WorkTime { get; protected set; }
    public int CleanedAmount { get; protected set; }
    public string State { get; protected set; }
    public int ItemsDateDayThreshold { get; protected set; }

    protected Cleaner(
        CleanerSchedule schedule,
        CleanerWorkTime workTime,
        int cleanedAmount,
        string state,
        int itemsDateDayThreshold,
        Guid? id = null
    )
    {
        Id = id ?? Guid.NewGuid();
        Schedule = schedule;
        WorkTime = workTime;
        CleanedAmount = cleanedAmount;
        State = state;
        ItemsDateDayThreshold = itemsDateDayThreshold;
    }

    protected Cleaner(Cleaner cleaner)
        : this(
            cleaner.Schedule,
            cleaner.WorkTime,
            cleaner.CleanedAmount,
            cleaner.State,
            cleaner.ItemsDateDayThreshold,
            cleaner.Id
        ) { }

    public abstract Status StartWork();

    public abstract Status StartWait();

    public abstract Status Disable();

    public abstract Status AdjustWorkTimeByTotalSeconds(long totalSeconds);

    public abstract Status ChangeItemsToCleanThreshold(int threshold);

    public abstract Status CleanItem();
}
