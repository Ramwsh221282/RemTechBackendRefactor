using Microsoft.Extensions.DependencyInjection;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Outbox;
using Quartz;

namespace ParsedAdvertisements.Adapters.Outbox;

public static class DependencyInjection
{
    public static void AddParsedAdvertisementsOutboxAdapter(this IServiceCollection services)
    {
        services.AddScoped<IParsedAdvertisementsOutboxCleaner, ParsedAdvertisementsOutboxCleaner>();
        services.AddScoped<IParsedAdvertisementsOutboxDeliverer, ParsedAdvertisementsOutboxDeliverer>();
        services.AddOutboxCleaner();
        services.AddOutboxProcessor();
        services.AddScoped<ParsedAdvertisementsOutboxDbContext>();
    }

    private static void AddOutboxProcessor(this IServiceCollection services)
    {
        var jobKey = new JobKey(nameof(ParsedAdvertisementsOutboxProcessor));
        services.AddQuartz(q =>
        {
            q.AddJob<ParsedAdvertisementsOutboxProcessor>(options => options.WithIdentity(jobKey));
            q.AddTrigger(tr =>
                tr.ForJob(jobKey).WithIdentity(nameof(ParsedAdvertisementsOutboxProcessor) + "-trigger")
                    .WithCronSchedule("0/5 * * * * ?"));
        });
    }

    private static void AddOutboxCleaner(this IServiceCollection services)
    {
        var jobKey = new JobKey(nameof(ParsedAdvertisementsOutboxCleaningProcessor));
        services.AddQuartz(q =>
        {
            q.AddJob<ParsedAdvertisementsOutboxCleaningProcessor>(options => options.WithIdentity(jobKey));
            q.AddTrigger(tr => tr.ForJob(jobKey)
                .WithIdentity(nameof(ParsedAdvertisementsOutboxCleaningProcessor) + "-trigger")
                .WithCronSchedule("0/5 * * * * ?"));
        });
    }
}