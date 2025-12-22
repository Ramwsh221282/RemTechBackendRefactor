using ParsersControl.Core.Parsers.Contracts;
using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Tests;

public sealed class FakeOnParserWorkStartedListener(Serilog.ILogger logger) : IOnParserStartedListener
{
    public Serilog.ILogger Logger { get; } = logger.ForContext<IOnParserStartedListener>();

    public Task Handle(SubscribedParser parser, CancellationToken ct = default)
    {
        Logger.Information("Parser work started.");
        return Task.CompletedTask;
    }
}