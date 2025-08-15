using Cleaners.Module.BackgroundJobs.StartingWaitingCleaner;
using Quartz;
using Scrapers.Module.Features.StartParser.Entrance;

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
        });
    }
}
