using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Cleaners.Adapter.Outbox;

public static class CleanersOutboxDependencyInjection
{
    public static void AddCleanersOutboxProcessor(this IServiceCollection services)
    {
        var jobkey = new JobKey(nameof(OutboxProcessorJob));

        services.AddQuartz(q =>
        {
            q.AddJob<OutboxProcessorJob>(options => options.WithIdentity(jobkey));
            q.AddTrigger(options =>
                options
                    .ForJob(jobkey)
                    .WithIdentity(nameof(OutboxProcessorJob) + "-trigger")
                    .WithCronSchedule("0/5 * * * * ?")
            );
        });

        services.AddQuartzHostedService(q =>
        {
            q.WaitForJobsToComplete = true;
            q.AwaitApplicationStarted = true;
            q.StartDelay = TimeSpan.FromSeconds(10);
        });
    }
}
