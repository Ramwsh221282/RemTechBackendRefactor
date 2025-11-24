using Microsoft.Extensions.DependencyInjection;
using Quartz;
using RemTech.SharedKernel.Infrastructure.Quartz;

namespace CompositionRoot.Shared;

public static class QuartzDependencyInjection
{
    public static void AddQuartzJobs(this IServiceCollection services)
    {
        services.AddCronScheduledJobs();
        services.AddQuartzHostedService(c =>
        {
            c.AwaitApplicationStarted = true;
            c.WaitForJobsToComplete = true;
        });
    }
}