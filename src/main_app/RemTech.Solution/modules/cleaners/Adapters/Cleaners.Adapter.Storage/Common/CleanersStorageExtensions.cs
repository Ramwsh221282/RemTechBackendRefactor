using Cleaners.Adapter.Storage.DataModels;
using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Aggregate.Decorators;
using Cleaners.Domain.Cleaners.ValueObjects;
using Microsoft.EntityFrameworkCore;
using RemTech.Core.Shared.Result;

namespace Cleaners.Adapter.Storage.Common;

public static class CleanersStorageExtensions
{
    public static async Task<Status<CleanerDataModel>> GetCleanerForUpdate(
        this CleanersDbContext context,
        Guid id,
        CancellationToken ct
    )
    {
        var cleaner = await context
            .Cleaners.FromSqlInterpolated(
                @$"SELECT * FROM cleaners_module.cleaners where ID = {id} FOR UPDATE"
            )
            .FirstOrDefaultAsync(ct);

        if (cleaner == null)
            return Error.NotFound("Чистильщик не найден.");

        context.Attach(cleaner);
        return cleaner;
    }

    public static async Task<Status<TransactionalCleanerDataModel>> GetLockedCleaner(
        this CleanersDbContext context,
        Guid id,
        CancellationToken ct = default
    )
    {
        var txn = await context.Database.BeginTransactionAsync(ct);

        var cleaner = await context
            .Cleaners.FromSqlInterpolated(
                @$"SELECT * FROM cleaners_module.cleaners where ID = {id} FOR UPDATE"
            )
            .FirstOrDefaultAsync(ct);

        if (cleaner == null)
            return Error.NotFound("Чистильщик не найден.");

        context.Attach(cleaner);
        return new TransactionalCleanerDataModel(cleaner, txn, context);
    }

    public static CleanerDataModel ConvertToDataModel(this Cleaner cleaner)
    {
        return new CleanerDataModel()
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

    public static Cleaner ConvertToDomainModel(this ICleanerDataModel dataModel)
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
