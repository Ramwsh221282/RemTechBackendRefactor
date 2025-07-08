using RemTech.Core.Shared.Functional;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Async;

public sealed class AsyncUpdateParser : IMaybeError, IMaybeParserId, IMaybeParser
{
    private readonly ParserIdBag _parserIdBag;
    private readonly ErrorBag _errorBag;
    private readonly MaybeBag<NotEmptyString> _stateString;
    private readonly MaybeBag<PositiveInteger> _waitDaysBag;
    private ParserBag _parser = new();

    public AsyncUpdateParser(Guid parserId, string state, int waitDays)
        : this(parserId)
    {
        Status<NotEmptyString> stateString = NotEmptyString.New(state);
        Status<PositiveInteger> positiveInteger = PositiveInteger.New(waitDays);
        _errorBag = ErrorBag.New(stateString, positiveInteger);
        _stateString = stateString;
        _waitDaysBag = positiveInteger;
    }

    public AsyncUpdateParser(Guid parserId, string? state = null, int? waitDays = null)
        : this(parserId)
    {
        if (state != null)
        {
            Status<NotEmptyString> statestring = NotEmptyString.New(state);
            _stateString = statestring;
            _errorBag = statestring.IsFailure ? new ErrorBag(statestring.Error) : new ErrorBag();
        }

        if (waitDays != null)
        {
            Status<PositiveInteger> posInteger = PositiveInteger.New(waitDays.Value);
            _waitDaysBag = posInteger;
            _errorBag = posInteger.IsFailure ? new ErrorBag(posInteger.Error) : new ErrorBag();
        }
    }

    public AsyncUpdateParser(Guid parserId, string state)
        : this(parserId)
    {
        Status<NotEmptyString> stateString = NotEmptyString.New(state);
        _errorBag = ErrorBag.New(stateString);
        _stateString = stateString;
    }

    public AsyncUpdateParser(Guid parser, int waitDays)
        : this(parser)
    {
        Status<PositiveInteger> positiveInteger = PositiveInteger.New(waitDays);
        _errorBag = ErrorBag.New(positiveInteger);
        _waitDaysBag = positiveInteger;
    }

    private AsyncUpdateParser(Guid parserId)
    {
        Status<NotEmptyGuid> guid = NotEmptyGuid.New(parserId);
        _parserIdBag = guid;
        _errorBag = ErrorBag.New(guid);
        _stateString = new MaybeBag<NotEmptyString>();
        _waitDaysBag = new MaybeBag<PositiveInteger>();
    }

    public NotEmptyString ReadStateString() => _stateString;

    public PositiveInteger ReadPositiveInteger() => _waitDaysBag;

    public MaybeBag<NotEmptyString> MaybeState() => _stateString;

    public MaybeBag<PositiveInteger> MaybeWaitDays() => _waitDaysBag;

    public bool Errored() => _errorBag.Errored();

    public Error Error() => _errorBag.Error();

    public void Put(IParser parser) => _parser.Put(parser);

    IParser IMaybeParser.Take() => _parser.Take();

    public NotEmptyGuid Take() => _parserIdBag.Take();

    public void Put(NotEmptyGuid parserId) => _parserIdBag.Take();
}
