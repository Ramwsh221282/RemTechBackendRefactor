using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Tests.ParsersControl.Features;

public sealed class ParserControlFeaturesFacade(IServiceProvider sp)
{
    private readonly AddParserFeature _addParser = new(sp);
    private readonly EnsureParserExists _ensureExists = new(sp);
    private readonly HasParserStatistic _hasStatistic = new(sp);

    public async Task<Result<AddParserResponse>> AddParser(string domain, string type)
    {
        return await _addParser.Invoke(domain, type);
    }

    public async Task<bool> EnsureParserExists(Guid id)
    {
        return await _ensureExists.Invoke(id);
    }

    public async Task<Result<ParserStateChangeResponse>> EnableParser(Guid id)
    {
        return await new EnableParserFeature(sp).Invoke(id);
    }

    public async Task<Result<ParserStateChangeResponse>> DisableParser(Guid id)
    {
        return await new DisableParserFeature(sp).Invoke(id);
    }
    
    public async Task<Result<ParserStateChangeResponse>> MakeWaitingParser(Guid id)
    {
        return await new MakeWaitingParserFeature(sp).Invoke(id);
    }

    public async Task<Result<ParserStateChangeResponse>> MakeWorkingParser(Guid id)
    {
        return await new MakeWorkingParserFeature(sp).Invoke(id);
    }
    
    public async Task<Result<ParserStateChangeResponse>> PermanentlyDisableParser(Guid id)
    {
        return await new PermanentlyDisableParserFeature(sp).Invoke(id);
    }

    public async Task<bool> HasParserState(Guid parserId)
    {
        return await new HasParserState(sp).Invoke(parserId);
    }
    
    public async Task<bool> HasParserStatistic(Guid parserId)
    {
        return await _hasStatistic.Invoke(parserId);
    }

    public async Task<Result<ParserStatisticsUpdateResponse>> UpdateProcessed(
        Guid id, 
        int processed)
    {
        return await new UpdateParserProcessedFeature(sp).Invoke(id, processed);
    }
    
    public async Task<Result<ParserStatisticsUpdateResponse>> UpdateElapsedSeconds(
        Guid id, 
        long elapsedSeconds)
    {
        return await new UpdateParserElapsedSecondsFeature(sp).Invoke(id, elapsedSeconds);
    }

    public async Task<bool> ProcessedEqualsTo(
        Guid id,
        int processed
        )
    {
        return await new ParserProcessedEqualsFeature(sp).Invoke(id, processed);
    }

    public async Task<bool> ElapsedEqualsTo(
        Guid id,
        long elapsedSeconds
        )
    {
        return await new ParserElapsedEqualsFeature(sp).Invoke(id, elapsedSeconds);
    }

    public async Task<bool> EnsureHasSchedule(Guid id)
    {
        return await new HasParserSchedule(sp).Invoke(id);
    }

    public async Task<Result<ParserScheduleUpdateResponse>> UpdateWaitDays(Guid id, int waitDays)
    {
        return await new UpdateWaitDaysFeature(sp).Invoke(id, waitDays);
    }

    public async Task<Result<ParserScheduleUpdateResponse>> SetFinishedAt(Guid id, DateTime finishedAt)
    {
        return await new SetFinishedFeature(sp).Invoke(id, finishedAt);
    }
}