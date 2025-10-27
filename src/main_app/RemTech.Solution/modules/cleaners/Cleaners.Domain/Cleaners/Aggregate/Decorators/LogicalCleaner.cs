using Cleaners.Domain.Cleaners.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.Aggregate.Decorators;

public sealed class LogicalCleaner : Cleaner
{
    public override Status UpdateSchedule(int waitDays)
    {
        if (State == WorkState)
            return Status.Conflict("Нельзя изменить расписание чистильщика в рабочем состоянии.");

        var updating = Schedule.UpdateSchedule(waitDays);
        if (updating.IsFailure)
            return updating;

        Schedule = updating;
        return Status.Success();
    }

    public override Status StartWork()
    {
        if (State == WorkState)
            return Status.Conflict("Чистильщик уже в рабочем состоянии.");

        CleanedAmount = 0;
        State = WorkState;
        Schedule = Schedule.Adjust();
        WorkTime = new CleanerWorkTime();
        return Status.Success();
    }

    public override Status StartWait()
    {
        if (State == WaitingState)
            return Status.Conflict("Чистильщик уже в ожидании.");

        State = WaitingState;
        return Status.Success();
    }

    public override Status Disable()
    {
        if (State == DisabledState)
            return Status.Conflict("Чистильщик уже в отключенном состоянии.");

        State = DisabledState;
        return Status.Success();
    }

    public override Status AdjustWorkTimeByTotalSeconds(long totalSeconds)
    {
        if (State != WorkState)
            return Status.Conflict(
                "Только в рабочем состоянии чистильщик может обновить затраченное время работы."
            );

        Status<CleanerWorkTime> workTime = CleanerWorkTime.Create(totalSeconds);
        if (workTime.IsFailure)
            return new Status(workTime.Error);

        WorkTime = workTime.Value;
        return Status.Success();
    }

    public override Status ChangeItemsToCleanThreshold(int threshold)
    {
        if (State == WorkState)
            return Status.Conflict("Нельзя редактировать чистильщика, пока он работает.");
        if (threshold < 1)
            return Status.Validation(
                "Свежесть объявлений, добавленных в агрегатор, не может быть днем меньше 1."
            );
        if (threshold > 7)
            return Status.Validation(
                "Свежесть объявлений, добавленных в агрегатор, не может быть более 7 дней."
            );

        ItemsDateDayThreshold = threshold;
        return Status.Success();
    }

    public override Status CleanItem()
    {
        if (State != WorkState)
            return Status.Conflict(
                "Только в рабочем состоянии, чистильщик может обновить количество обработанных записей."
            );
        CleanedAmount += 1;
        return Status.Success();
    }

    public override Status UpdateWorkStatistics(CleanerWorkTime time, int processedAmount)
    {
        if (State != WorkState)
            return Status.Conflict(
                "Только в рабочем состоянии чистильщик может обновить статистику работы"
            );

        WorkTime = time;
        CleanedAmount = processedAmount;
        return Status.Success();
    }

    private LogicalCleaner(
        CleanerSchedule schedule,
        CleanerWorkTime workTime,
        int cleanedAmount,
        string state,
        int itemsDateDayThreshold,
        Guid? id = null
    )
        : base(schedule, workTime, cleanedAmount, state, itemsDateDayThreshold, id) { }

    public static Status<LogicalCleaner> Create(
        CleanerSchedule schedule,
        CleanerWorkTime workTime,
        int cleanedAmount,
        string state,
        int itemsDateDayThreshold,
        Guid? id = null
    )
    {
        if (cleanedAmount < 0)
            return Error.Validation("Количество очищенных объявлений не может быть отрицательным");
        if (!AllowedStates.Any(s => s == state))
            return Error.Validation($"Состояние чистильщика {state} не поддерживается.");
        if (itemsDateDayThreshold < 1)
            return Error.Validation(
                "Свежесть объявлений, добавленных в агрегатор, не может быть днем меньше 1."
            );
        if (itemsDateDayThreshold > 7)
            return Error.Validation(
                "Свежесть объявлений, добавленных в агрегатор, не может быть более 7 дней."
            );
        LogicalCleaner cleaner = new LogicalCleaner(
            schedule,
            workTime,
            cleanedAmount,
            state,
            itemsDateDayThreshold,
            id
        );

        return cleaner;
    }
}
