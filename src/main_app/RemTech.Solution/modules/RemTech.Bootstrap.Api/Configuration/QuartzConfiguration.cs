using Quartz;
using RemTech.ContainedItems.Module.BackgroundJobs.RemoveMarkedItems;

namespace RemTech.Bootstrap.Api.Configuration;

public static class QuartzConfiguration
{
    public static void ConfigureQuartz(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            JobKey jobKey = JobKey.Create(nameof(StartingParsersEntrance));
            options
                .AddJob<StartingParsersEntrance>(jobKey)
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(jobKey)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInSeconds(30).RepeatForever()
                        )
                );

            JobKey cleanerJob = JobKey.Create(nameof(StartWaitingCleanerJob));
            options
                .AddJob<StartWaitingCleanerJob>(cleanerJob)
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(cleanerJob)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInSeconds(30).RepeatForever()
                        )
                );

            JobKey itemRemoverJob = JobKey.Create(nameof(RemoveMarkedItemsJob));
            options
                .AddJob<RemoveMarkedItemsJob>(itemRemoverJob)
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(itemRemoverJob)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInMinutes(1).RepeatForever()
                        )
                );
        });
    }
}
