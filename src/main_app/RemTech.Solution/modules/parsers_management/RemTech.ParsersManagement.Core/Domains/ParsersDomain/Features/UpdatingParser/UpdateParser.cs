using RemTech.Core.Shared.Functional;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser;

public sealed class UpdateParser : IMaybeError, IMaybeParser
{
    private readonly MaybeBag<NotEmptyString> _stateBag;
    private readonly MaybeBag<PositiveInteger> _waitDaysBag;
    private readonly ErrorBag _errorBag;
    private IParser _parser;

    public UpdateParser(IParser parser, NotEmptyString state)
        : this(parser) => _stateBag = state;

    public UpdateParser(IParser parser, PositiveInteger waitDays)
        : this(parser) => _waitDaysBag = waitDays;

    public UpdateParser(IParser parser, PositiveInteger waitDays, NotEmptyString state)
        : this(parser)
    {
        _stateBag = state;
        _waitDaysBag = waitDays;
    }

    public UpdateParser(IParser parser, NotEmptyString state, PositiveInteger waitDays)
        : this(parser)
    {
        _stateBag = state;
        _waitDaysBag = waitDays;
    }

    public UpdateParser(IParser parser, string state)
        : this(parser)
    {
        Status<NotEmptyString> stateString = NotEmptyString.New(state);
        _stateBag = stateString;
        _errorBag = ErrorBag.New(stateString);
    }

    public UpdateParser(IParser parser, int waitDays)
        : this(parser)
    {
        Status<PositiveInteger> posInteger = PositiveInteger.New(waitDays);
        _waitDaysBag = posInteger;
        _errorBag = ErrorBag.New(posInteger);
    }

    public UpdateParser(IParser parser, string? state = null, int? waitDays = null)
        : this(parser)
    {
        if (state != null)
        {
            Status<NotEmptyString> stateString = NotEmptyString.New(state);
            _stateBag = stateString;
            _errorBag = stateString.IsFailure ? new ErrorBag(stateString.Error) : new ErrorBag();
        }

        if (waitDays != null)
        {
            Status<PositiveInteger> posInteger = PositiveInteger.New(waitDays.Value);
            _waitDaysBag = posInteger;
            _errorBag = posInteger.IsFailure ? new ErrorBag(posInteger.Error) : new ErrorBag();
        }
    }

    public UpdateParser(
        IParser parser,
        MaybeBag<NotEmptyString> state,
        MaybeBag<PositiveInteger> waitDays
    )
        : this(parser)
    {
        _stateBag = state;
        _waitDaysBag = waitDays;
    }

    public UpdateParser(IParser parser)
    {
        _parser = parser;
        _stateBag = new MaybeBag<NotEmptyString>();
        _waitDaysBag = new MaybeBag<PositiveInteger>();
        _errorBag = new ErrorBag();
    }

    public bool AnyWaitDays() => _waitDaysBag.Any();

    public PositiveInteger WaitDays() => _waitDaysBag.Take();

    public bool AnyState() => _stateBag.Any();

    public NotEmptyString TakeState() => _stateBag.Take();

    public void Put(IParser parser) => _parser = parser;

    public IParser Take() => _parser;

    public bool Errored() => _errorBag.Errored();

    public Error Error() => _errorBag.Error();
}
