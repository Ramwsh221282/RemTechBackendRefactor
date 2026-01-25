using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Tests.Parsers.SubscribeParser;

public sealed class FakeOnParserSubscribedListener(Serilog.ILogger logger) : IOnParserSubscribedListener
{
    public Serilog.ILogger Logger { get; } = logger.ForContext<IOnParserSubscribedListener>();

    public Task Handle(SubscribedParser parser, CancellationToken ct = default)
    {
        Logger.Information("Parser subscribed.");
        return Task.CompletedTask;
    }
}