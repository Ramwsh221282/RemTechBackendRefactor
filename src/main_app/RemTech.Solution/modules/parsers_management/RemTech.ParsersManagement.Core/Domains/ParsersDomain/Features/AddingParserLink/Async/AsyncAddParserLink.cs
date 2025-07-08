using RemTech.ParsersManagement.Core.Common.Errors;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Async;

public sealed class AsyncAddParserLink : IMaybeError, IMaybeParser, IMaybeParserId
{
    private readonly Status<NotEmptyGuid> _parserId;
    private readonly Status<NotEmptyString> _linkUrl;
    private readonly Status<NotEmptyString> _linkName;
    private readonly ErrorBag _error;
    private readonly ParserBag _parser;

    public AsyncAddParserLink(Guid? parserId, string? linkName, string? linkUrl)
    {
        _parserId = NotEmptyGuid.New(parserId);
        _linkUrl = NotEmptyString.New(linkUrl);
        _linkName = NotEmptyString.New(linkName);
        _error = ErrorBag.New(_parserId, _linkUrl, _linkName);
        _parser = new ParserBag();
    }

    public NotEmptyGuid TakeOwnerId() => _parserId;

    public NotEmptyString TakeLinkUrl() => _linkUrl;

    public NotEmptyString TakeLinkName() => _linkName;

    public bool Errored() => _error.Errored();

    public Error Error() => _error.Error();

    public void Put(IParser parser) => _parser.Put(parser);

    public IParser Take() => _parser.Take();

    public void Put(NotEmptyGuid parserId) { }

    public bool Has() => _parser.Has();

    NotEmptyGuid IMaybeParserId.Take() => _parserId;
}
