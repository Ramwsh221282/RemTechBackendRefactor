using RemTech.ParsersManagement.Core.Common.Errors;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async;

public sealed class AsyncDisableParser : IMaybeError, IMaybeParser, IMaybeParserId
{
    private readonly ParserBag _parserBag;
    private readonly ParserIdBag _idBag;
    private readonly ErrorBag _errorBag;

    public AsyncDisableParser(Guid? id)
    {
        Status<NotEmptyGuid> guid = NotEmptyGuid.New(id);
        _errorBag = ErrorBag.New(guid);
        _parserBag = new ParserBag();
        _idBag = guid.IsFailure ? new ParserIdBag() : new ParserIdBag(guid);
    }

    public bool Errored() => _errorBag.Errored();

    public Error Error() => _errorBag.Error();

    public NotEmptyGuid WhomDisableId() => _idBag.Take();

    public void Put(IParser parser) => _parserBag.Put(parser);

    public IParser Take() => _parserBag.Take();

    public void Put(NotEmptyGuid parserId) => _idBag.Put(parserId);

    NotEmptyGuid IMaybeParserId.Take() => _idBag.Take();
}
