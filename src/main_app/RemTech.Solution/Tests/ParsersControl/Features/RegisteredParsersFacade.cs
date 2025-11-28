using ParsersControl.Presenters.ParserRegistrationManagement.AddParser;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Tests.ParsersControl.Features;

public sealed class RegisteredParsersFacade(IServiceProvider sp)
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
}