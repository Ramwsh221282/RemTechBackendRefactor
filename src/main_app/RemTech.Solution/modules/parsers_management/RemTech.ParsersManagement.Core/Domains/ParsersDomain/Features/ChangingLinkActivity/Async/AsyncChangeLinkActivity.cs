using RemTech.ParsersManagement.Core.Common.Errors;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Async;

public sealed class AsyncChangeLinkActivity : IMaybeError, IMaybeParser
{
    private readonly ParserBag _parser;
    private readonly ErrorBag _error;
    private readonly Status<NotEmptyGuid> _linkId;
    private readonly Status<NotEmptyGuid> _parserId;
    private readonly bool _nextActivity;

    public AsyncChangeLinkActivity(Guid? parserId, Guid? linkId, bool nextActivity)
    {
        _parserId = NotEmptyGuid.New(parserId);
        _linkId = NotEmptyGuid.New(linkId);
        _error = ErrorBag.New(_parserId, _linkId);
        _parser = new ParserBag();
        _nextActivity = nextActivity;
    }

    public bool NextActivity() => _nextActivity;

    public NotEmptyGuid WhomChange() => _linkId;

    public NotEmptyGuid OwnerId() => _parserId;

    public bool Errored() => _error.Errored();

    public Error Error() => _error.Error();

    public void Put(IParser parser) => _parser.Put(parser);

    public IParser Take() => _parser.Take();
}
