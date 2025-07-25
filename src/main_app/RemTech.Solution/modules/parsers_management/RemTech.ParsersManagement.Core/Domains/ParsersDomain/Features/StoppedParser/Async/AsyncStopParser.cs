﻿using RemTech.Core.Shared.Functional;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser.Async;

public sealed class AsyncStopParser : IMaybeError, IMaybeParser
{
    private readonly ErrorBag _error;
    private readonly ParserBag _parser;
    private readonly Status<NotEmptyGuid> _parserId;

    public AsyncStopParser(Guid? parserId)
    {
        _parserId = NotEmptyGuid.New(parserId);
        _error = ErrorBag.New(_parserId);
        _parser = new ParserBag();
    }

    public NotEmptyGuid TakeWhomStopId() => _parserId.Value;

    public bool Errored() => _error.Errored();

    public Error Error() => _error.Error();

    public void Put(IParser parser) => _parser.Put(parser);

    public IParser Take() => _parser.Take();
}
