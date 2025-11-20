using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Identity.Outbox;

public static class IdentityOutboxProcessorExtensions
{
    extension(IServiceCollection services)
    {
        public void AddIdentityOutboxProcessor()
        {
            services.AddSingleton<IdentityOutboxProcessorWork>();
            services.AddSingleton<IdentityOutboxProcessor>();
            JobKey key = new(nameof(IdentityOutboxProcessor));
            services.AddQuartz(q =>
            {
                q.AddJob<IdentityOutboxProcessor>(c =>
                {
                    c.StoreDurably();
                    c.WithIdentity(key);
                }).AddTrigger(t =>
                {
                    t.ForJob(key).WithCronSchedule("*/5 * * * * ?");
                    t.WithIdentity($"trigger_{nameof(IdentityOutboxProcessor)}");
                });
            });
        }
    }
}