using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Cleaner.Adapter.JobScheduler;

public static class CleanerInvokeJobDependencyInjection
{
    public static void AddCleanerJob(this IServiceCollection services)
    {
        var jobKey = new JobKey(nameof(CleanerInvokeJob));

        services.AddQuartz(q =>
        {
            q.AddJob<CleanerInvokeJob>(options => options.WithIdentity(jobKey));
            q.AddTrigger(trigger =>
            {
                trigger
                    .ForJob(jobKey)
                    .WithIdentity(nameof(CleanerInvokeJob) + "-trigger")
                    .WithCronSchedule("0/5 * * * * ?");
            });
        });
    }
}
