using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Async;

public sealed class AsyncEnableParser
{
    private readonly EnableParser _enable = new();
    private readonly Status<NotEmptyGuid> _id;
    private readonly Error _error;

    public AsyncEnableParser(Guid? id)
    {
        _id = NotEmptyGuid.New(id);
        _error = _id.IsFailure ? _id.Error : Result.Library.Error.None();
    }

    public NotEmptyGuid WhomEnable() => _id.Value;

    public bool Errored()
    {
        bool any = _error.Any();
        return any;
    }

    public Error Error() => _error;

    public void PutParser(IParser parser) => _enable.PutParser(parser);

    public EnableParser Enable() => _enable;
}
