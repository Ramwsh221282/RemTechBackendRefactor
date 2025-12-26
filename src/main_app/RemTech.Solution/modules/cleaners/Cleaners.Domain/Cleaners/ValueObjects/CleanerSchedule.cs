using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.ValueObjects;

public readonly record struct CleanerSchedule
{
    private const int MaxWaitDaysInterval = 7;
    private const int MinWaitDaysInterval = 1;

    public DateTime? LastRun { get; }
    public DateTime? NextRun { get; }
    public int WaitDays { get; }

    public CleanerSchedule()
    {
        LastRun = null;
        NextRun = null;
        WaitDays = 1;
    }

    private CleanerSchedule(DateTime? lastRun, DateTime? nextRun, int waitDays)
    {
        LastRun = lastRun;
        NextRun = nextRun;
        WaitDays = waitDays;
    }

    public Status<CleanerSchedule> UpdateSchedule(int waitDays)
    {
        DateTime lastRun = LastRun ?? DateTime.UtcNow;
        DateTime nextRun = lastRun.AddDays(waitDays);
        return Create(lastRun, nextRun, waitDays);
    }

    public CleanerSchedule Adjust()
    {
        DateTime lastRun = DateTime.UtcNow;
        DateTime nextRun = lastRun.AddDays(WaitDays);
        return new CleanerSchedule(lastRun, nextRun, WaitDays);
    }

    public Status<CleanerSchedule> ChangeWaitDays(int newWaitDays) =>
        Create(LastRun, NextRun, newWaitDays);

    public static Status<CleanerSchedule> Create(DateTime? lastRun, DateTime? nextRun, int waitDays)
    {
        if (lastRun != null && lastRun == DateTime.MinValue || lastRun == DateTime.MaxValue)
            Error.Validation("Дата последнего запуска некорректная.");
        if (nextRun != null && nextRun == DateTime.MinValue || nextRun == DateTime.MaxValue)
            return Error.Validation("Дата следующего запуска некорректная");
        if (nextRun != null && lastRun == null)
            return Error.Validation(
                "Некорректные даты расписания. Не может быть даты следующего запуска, без даты последнего запуска."
            );
        if (nextRun != null && lastRun != null)
            if (lastRun > nextRun)
                return Error.Validation(
                    "Некорректные даты расписания. Дата последнего запуска не может быть даты следующего запуска."
                );
        if (waitDays < MinWaitDaysInterval)
            return Error.Validation($"Дни ожидания не могут быть менее {MinWaitDaysInterval}");
        if (waitDays > MaxWaitDaysInterval)
            return Error.Validation($"Дни ожидания не могут быть более {MaxWaitDaysInterval}");
        if (nextRun != null && lastRun != null)
        {
            if (lastRun > nextRun)
                return Error.Validation(
                    "Некорректные даты расписания. Дата последнего запуска не может быть позже даты следующего запуска."
                );

            var diffDays = (nextRun.Value - lastRun.Value).TotalDays;

            if (diffDays <= 0)
                return Error.Validation(
                    "Разница между датой последнего и следующего запуска должна быть не менее 1 дня."
                );

            if (diffDays > MaxWaitDaysInterval)
                return Error.Validation(
                    "Разница между датой последнего и следующего запуска не должна превышать 7 дней."
                );
        }
        return new CleanerSchedule(lastRun, nextRun, waitDays);
    }
}
