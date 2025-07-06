using RemTech.ParsersManagement.Core.Common.Errors;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed.Async;

public sealed class AsyncIncreaseProcess : IMaybeParser, IMaybeError
{
    private readonly ParserBag _parser;
    private readonly ErrorBag _error;
    private readonly Status<NotEmptyGuid> _parserId;
    private readonly Status<NotEmptyGuid> _linkId;

    public AsyncIncreaseProcess(Guid? parserId, Guid? linkId)
    {
        _parserId = NotEmptyGuid.New(parserId);
        _linkId = NotEmptyGuid.New(linkId);
        _error = ErrorBag.New(_parserId, _linkId);
        _parser = new ParserBag();
    }

    public NotEmptyGuid TakeOwnerId() => _parserId;

    public NotEmptyGuid WhomIncreaseId() => _linkId;

    public void Put(IParser parser) => _parser.Put(parser);

    public IParser Take() => _parser.Take();

    public bool Errored() => _error.Errored();

    public Error Error() => _error.Error();
}
