using ContainedItems.Infrastructure.Queries.GetMainPageLastAddedItems;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ContainedItems.Infrastructure.BackgroundServices;

public sealed class MainPageLastAddedItemsInvalidationBackgroundService(
    Serilog.ILogger logger,
    IServiceProvider serviceProvider
) : BackgroundService
{
    private Serilog.ILogger Logger { get; } = logger.ForContext<MainPageStatsCacheInvalidationBackgroundService>();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Execute(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task Execute(CancellationToken ct)
    {
        try
        {
            logger.Information("Invalidating main page last added items cache.");
            await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
            HybridCache cache = scope.ServiceProvider.GetRequiredService<HybridCache>();
            string key = BackgroundServicesUtils.CreateKey(new GetMainPageLastAddedItemsQuery());
            await cache.RemoveAsync(key, ct);
            logger.Information("Main page last added items invalidated.");
        }
        catch(Exception ex)
        {
            Logger.Fatal(ex, "Error invalidating main page stats cache.");
        }
    }
}