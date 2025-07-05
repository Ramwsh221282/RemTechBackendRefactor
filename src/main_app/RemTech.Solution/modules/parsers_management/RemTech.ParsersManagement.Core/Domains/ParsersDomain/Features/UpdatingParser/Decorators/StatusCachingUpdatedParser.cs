using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Decorators;

public sealed class StatusCachingUpdatedParser(IUpdatedParser inner) : IUpdatedParser
{
    private MaybeBag<Status<IParser>> _processed = new();

    public Status<IParser> Updated(UpdateParser update)
    {
        if (_processed.Any())
            return _processed.Take();
        Status<IParser> processed = inner.Updated(update);
        _processed = _processed.Put(processed);
        return processed;
    }
}
