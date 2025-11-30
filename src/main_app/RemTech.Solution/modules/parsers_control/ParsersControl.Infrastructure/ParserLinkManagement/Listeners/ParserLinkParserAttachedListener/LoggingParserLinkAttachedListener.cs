using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Infrastructure.ParserLinkManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkParserAttachedListener;

public sealed class LoggingParserLinkAttachedListener(Serilog.ILogger logger) : IParserLinkParserAttached
{
    private readonly Serilog.ILogger _logger = logger.ForContext<IParserLinkParserAttached>();
    
    public Task<Result<Unit>> React(Guid parserId, ParserLinkData data, CancellationToken ct = default)
    {
        _logger.Information("Parser attached to parser link.");
        ParserLinkDataLog log = new(new ParserLink(data), _logger);
        log.Log();
        return Task.FromResult(Result.Success(Unit.Value));
    }
}