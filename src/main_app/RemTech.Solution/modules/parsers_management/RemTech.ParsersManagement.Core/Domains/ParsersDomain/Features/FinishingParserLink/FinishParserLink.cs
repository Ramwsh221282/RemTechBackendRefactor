using RemTech.ParsersManagement.Core.Common.Errors;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink;

public sealed class FinishParserLink : IMaybeError
{
    private readonly IParser _parser;
    private readonly Status<PositiveLong> _elapsed;
    private readonly Status<NotEmptyGuid> _linkId;
    private readonly ErrorBag _errorBag;

    public FinishParserLink(IParser parser, Guid? linkId, long? elapsed)
    {
        _parser = parser;
        _elapsed = PositiveLong.New(elapsed);
        _linkId = NotEmptyGuid.New(linkId);
        _errorBag = ErrorBag.New(_elapsed, _linkId);
    }

    public bool Errored() => _errorBag.Errored();

    public Error Error() => _errorBag.Error();

    public IParser TakeOwner() => _parser;

    public PositiveLong HowMuchTaken() => _elapsed.Value;

    public NotEmptyGuid WhoEnded() => _linkId.Value;
}
