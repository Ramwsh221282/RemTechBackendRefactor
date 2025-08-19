using Scrapers.Module.Domain.JournalsContext;
using Scrapers.Module.Domain.JournalsContext.Features.CreateScraperJournal;
using Scrapers.Module.Features.InstantlyEnableParser.Endpoint;

namespace Scrapers.Module.Features.InstantlyEnableParser.Models;

internal sealed class InstantlyEnabledParser
{
    private readonly string _name;
    private readonly string _type;
    private readonly string _state;
    private readonly DateTime _nextRun;
    private readonly DateTime _lastRun;

    public InstantlyEnabledParser(string name, string type)
    {
        _name = name;
        _type = type;
        _state = "Работает";
        DateTime utcNow = DateTime.UtcNow;
        _nextRun = utcNow;
        _lastRun = utcNow;
    }

    public InstantlyEnabledParserLogMessage LogMessage()
    {
        return new InstantlyEnabledParserLogMessage(_name, _type);
    }

    public async Task Save(IInstantlyEnabledParsersStorage storage, CancellationToken ct = default)
    {
        await storage.Save(_name, _type, _state, _nextRun, _lastRun, ct);
    }

    public InstantlyEnableParserEndpoint.InstantlyEnabledParserResponse CreateResponse()
    {
        return new InstantlyEnableParserEndpoint.InstantlyEnabledParserResponse(
            _name,
            _type,
            _state,
            _lastRun,
            _nextRun
        );
    }

    public CreateScraperJournalCommand CreateJournalCommand()
    {
        return new CreateScraperJournalCommand(_name, _type);
    }
}
