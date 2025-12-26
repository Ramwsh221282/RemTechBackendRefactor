using Cleaners.WebApi.Responses;
using RemTech.Core.Shared.Result;

namespace Cleaners.WebApi.Extensions;

public static class CleanerWebApiExtensions
{
    public static CleanerDto ToDto(this Domain.Cleaners.Aggregate.Cleaner cleaner)
    {
        return new CleanerDto()
        {
            Id = cleaner.Id,
            Threshold = cleaner.ItemsDateDayThreshold,
            ProcessedAmount = cleaner.CleanedAmount,
            LastRun = cleaner.Schedule.LastRun,
            NextRun = cleaner.Schedule.NextRun,
            WaitDays = cleaner.Schedule.WaitDays,
            ElapsedSeconds = cleaner.WorkTime.ElapsedSeconds,
            ElapsedHours = cleaner.WorkTime.ElapsedHours,
            ElapsedMinutes = cleaner.WorkTime.ElapsedMinutes,
            State = cleaner.State,
        };
    }

    public static CleanerDto ToDto(this Status<Domain.Cleaners.Aggregate.Cleaner> cleaner)
    {
        return new CleanerDto()
        {
            Id = cleaner.Value.Id,
            Threshold = cleaner.Value.ItemsDateDayThreshold,
            ProcessedAmount = cleaner.Value.CleanedAmount,
            LastRun = cleaner.Value.Schedule.LastRun,
            NextRun = cleaner.Value.Schedule.NextRun,
            WaitDays = cleaner.Value.Schedule.WaitDays,
            ElapsedSeconds = cleaner.Value.WorkTime.ElapsedSeconds,
            ElapsedHours = cleaner.Value.WorkTime.ElapsedHours,
            ElapsedMinutes = cleaner.Value.WorkTime.ElapsedMinutes,
            State = cleaner.Value.State,
        };
    }
}
