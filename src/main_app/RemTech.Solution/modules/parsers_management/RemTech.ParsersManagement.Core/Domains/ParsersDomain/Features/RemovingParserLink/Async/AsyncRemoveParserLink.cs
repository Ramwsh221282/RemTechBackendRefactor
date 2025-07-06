using RemTech.ParsersManagement.Core.Common.Errors;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink.Async;

public sealed class AsyncRemoveParserLink : IMaybeParser, IMaybeError
{
    private readonly ErrorBag _error;
    private readonly ParserBag _parser;
    private readonly Status<NotEmptyGuid> _parserId;
    private readonly Status<NotEmptyGuid> _linkId;

    public AsyncRemoveParserLink(Guid? parserId, Guid? linkId)
    {
        _parserId = NotEmptyGuid.New(parserId);
        _linkId = NotEmptyGuid.New(linkId);
        _error = ErrorBag.New(_parserId, _linkId);
        _parser = new ParserBag();
    }

    public NotEmptyGuid TakeOwnerId() => _parserId;

    public NotEmptyGuid WhomRemoveId() => _linkId;

    public void Put(IParser parser) => _parser.Put(parser);

    public IParser Take() => _parser.Take();

    public bool Errored() => _error.Errored();

    public Error Error() => _error.Error();
}
