using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Aggregate.Decorators;
using Cleaners.Domain.Cleaners.ValueObjects;

namespace Cleaners.Adapters.Cache;

public static class CleanersCachedStorageExtensions
{
    public static CleanerCachedModel ConvertToDataModel(this Cleaner cleaner)
    {
        return new CleanerCachedModel()
        {
            Id = cleaner.Id,
            CleanedAmount = cleaner.CleanedAmount,
            State = cleaner.State,
            ItemsDateDayThreshold = cleaner.ItemsDateDayThreshold,
            LastRun = cleaner.Schedule.LastRun,
            NextRun = cleaner.Schedule.NextRun,
            WaitDays = cleaner.Schedule.WaitDays,
            Hours = cleaner.WorkTime.ElapsedHours,
            Minutes = cleaner.WorkTime.ElapsedMinutes,
            Seconds = cleaner.WorkTime.ElapsedSeconds,
        };
    }

    public static Cleaner ConvertToDomainModel(this CleanerCachedModel dataModel)
    {
        var schedule = CleanerSchedule.Create(
            dataModel.LastRun,
            dataModel.NextRun,
            dataModel.WaitDays
        );

        if (schedule.IsFailure)
            throw new ApplicationException(
                $"Unable to create domain model from data model. Error: {schedule.Error.ErrorText}"
            );

        var workTime = CleanerWorkTime.Create(
            dataModel.Seconds,
            dataModel.Minutes,
            dataModel.Hours
        );

        if (workTime.IsFailure)
            throw new ApplicationException(
                $"Unable to create domain model from data model. Error: {workTime.Error.ErrorText}"
            );

        var cleaner = LogicalCleaner.Create(
            schedule,
            workTime,
            dataModel.CleanedAmount,
            dataModel.State,
            dataModel.ItemsDateDayThreshold,
            dataModel.Id
        );

        if (cleaner.IsFailure)
            throw new ApplicationException(
                $"Unable to create domain model from data model. Error: {cleaner.Error.ErrorText}"
            );

        return cleaner;
    }
}