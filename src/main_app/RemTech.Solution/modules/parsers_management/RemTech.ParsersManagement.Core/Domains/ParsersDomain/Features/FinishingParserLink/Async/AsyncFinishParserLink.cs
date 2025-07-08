using RemTech.Core.Shared.Functional;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Async;

public sealed class AsyncFinishParserLink : IMaybeParser, IMaybeError
{
    private readonly ParserBag _parser;
    private readonly ErrorBag _error;
    private readonly Status<NotEmptyGuid> _parserId;
    private readonly Status<NotEmptyGuid> _linkId;
    private readonly Status<PositiveLong> _elapsed;

    public AsyncFinishParserLink(Guid? parserId, Guid? linkId, long? elapsed)
    {
        _parserId = NotEmptyGuid.New(parserId);
        _linkId = NotEmptyGuid.New(linkId);
        _elapsed = PositiveLong.New(elapsed);
        _parser = new ParserBag();
        _error = ErrorBag.New(_parserId, _linkId, _elapsed);
    }

    public PositiveLong HowMuchTaken() => _elapsed;

    public NotEmptyGuid WhatOwnerId() => _parserId;

    public NotEmptyGuid WhomFinishId() => _linkId;

    public void Put(IParser parser) => _parser.Put(parser);

    public IParser Take() => _parser.Take();

    public bool Errored() => _error.Errored();

    public Error Error() => _error.Error();
}
