using System.Diagnostics;
using Cleaner.Cache;
using Cleaner.Exceptions;
using Cleaner.RabbitMq;
using PuppeteerSharp;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Cleaner.Cleaning;

internal sealed class SuspiciousItemInspector(
    Serilog.ILogger logger,
    IPage page,
    IConnection connection,
    ConnectionMultiplexer multiplexer
)
{
    private readonly Stopwatch _workTimer = Stopwatch.StartNew();

    public async Task Inspect(SuspiciousItem item)
    {
        ICheckChallenge challenge = item.SpecifyChallenge(page, logger);
        try
        {
            await challenge.Process();
        }
        catch (ItemDoesNotPresentException ex)
        {
            ex.Log(logger);
            await ex.Publish(connection);
        }
    }

    public async Task FinishJob()
    {
        JobFinishedEvent @event = new(connection, _workTimer);
        await @event.Publish();
    }

    public async Task<bool> WasPermantlyDisabled()
    {
        CleanerStateCache cache = new(multiplexer);
        return await cache.IsStateDisabled();
    }
}
