using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser;

public sealed class UpdateParserDetails
{
    private readonly MaybeBag<NotEmptyString> _stateString;
    private readonly MaybeBag<PositiveInteger> _waitDays;
    private MaybeBag<IParser> _parser = new();

    public UpdateParserDetails(string? state, int? waitDays)
    {
        _stateString = new MaybeBag<NotEmptyString>().MaybePut(
            state,
            stateString => !string.IsNullOrWhiteSpace(stateString),
            stateString => NotEmptyString.New(stateString)
        );
        _waitDays = new MaybeBag<PositiveInteger>().MaybePut(
            waitDays,
            days => days != null,
            days => PositiveInteger.New(days)
        );
    }

    public MaybeBag<NotEmptyString> ProposedState() => _stateString;

    public MaybeBag<PositiveInteger> ProposedWaitDays() => _waitDays;

    public void PutParser(IParser parser) => _parser = _parser.Put(parser);

    public IParser Parser() => _parser.Take();
}
