using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Shared.Infrastructure.Module.Quartz;

public static class AddQuartzService
{
    public static void AddQuartsHostedService(this IServiceCollection services)
    {
        services.AddQuartzHostedService(q =>
        {
            q.WaitForJobsToComplete = true;
            q.AwaitApplicationStarted = true;
            q.StartDelay = TimeSpan.FromSeconds(10);
        });
    }
}
