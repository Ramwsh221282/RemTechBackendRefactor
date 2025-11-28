using ParsersControl.Presenters.ParserRegistrationManagement.AddParser;
using ParsersControl.Presenters.ParserStateManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Tests.ParsersControl.Features;

public sealed class ParserControlFeaturesFacade(IServiceProvider sp)
{
    private readonly AddParserFeature _addParser = new(sp);
    private readonly EnsureParserExists _ensureExists = new(sp);

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
}