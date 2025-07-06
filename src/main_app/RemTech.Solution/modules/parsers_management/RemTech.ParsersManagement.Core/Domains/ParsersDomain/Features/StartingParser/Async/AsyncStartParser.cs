using RemTech.ParsersManagement.Core.Common.Errors;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Async;

public sealed class AsyncStartParser : IMaybeError, IMaybeParser
{
    private readonly ErrorBag _error;
    private readonly ParserBag _parser;
    private readonly Status<NotEmptyGuid> _parserId;

    public AsyncStartParser(Guid? parserId)
    {
        _parserId = NotEmptyGuid.New(parserId);
        _error = ErrorBag.New(_parserId);
        _parser = new ParserBag();
    }

    public NotEmptyGuid WhomStartId() => _parserId;

    public bool Errored() => _error.Errored();

    public Error Error() => _error.Error();

    public void Put(IParser parser) => _parser.Put(parser);

    public IParser Take() => _parser.Take();
}
