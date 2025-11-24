using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace RemTech.SharedKernel.Infrastructure.Quartz;

public static class QuartzOutboxDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddCronScheduledJobs()
        {
            ServiceDescriptor[] jobs = services.Where(s => s.ServiceType == typeof(ICronScheduleJob)).ToArray();
            foreach (var job in jobs)
                services.AddQuartz(q => job.AddJobInQuartz(q));
        }
    }

    extension(ServiceDescriptor descriptor)
    {
        private void AddJobInQuartz(IServiceCollectionQuartzConfigurator configurator)
        {
            Type jobType = descriptor.ImplementationType!;
            CronScheduleAttribute? cronSchedule = jobType.GetCustomAttribute<CronScheduleAttribute>();
            if (cronSchedule == null)
                throw new ApplicationException($"Job: {jobType.Name} has not Cron Schedule Attribute");
            
            JobKey jobKey = new(jobType.Name);
            TriggerKey trigger = new($"trigger_{jobType.Name}");
            
            configurator.AddJob(jobType, jobKey, c =>
            {
                c.StoreDurably().WithIdentity(jobKey);
            });
            configurator.AddTrigger(t =>
            {
                t.ForJob(jobKey).WithIdentity(trigger).WithCronSchedule(cronSchedule.Schedule);
            });
        }
    }
}