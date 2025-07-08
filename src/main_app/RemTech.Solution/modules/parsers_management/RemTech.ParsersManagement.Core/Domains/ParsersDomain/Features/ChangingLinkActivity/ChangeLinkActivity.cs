using RemTech.Core.Shared.Functional;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity;

public sealed class ChangeLinkActivity : IMaybeError
{
    private readonly IParser _parser;
    private readonly Status<NotEmptyGuid> _linkId;
    private readonly bool _nextActivity;
    private readonly ErrorBag _error;

    public ChangeLinkActivity(IParser parser, IParserLink link, bool nextActivity)
        : this(parser, link.Identification().ReadId(), nextActivity) { }

    public ChangeLinkActivity(IParser parser, Guid? linkId, bool nextActivity)
    {
        _parser = parser;
        _nextActivity = nextActivity;
        _linkId = NotEmptyGuid.New(linkId);
        _error = ErrorBag.New(_linkId);
    }

    public IParser TakeOwner() => _parser;

    public NotEmptyGuid TakeWhatToChange() => _linkId;

    public bool TakeNextActivity() => _nextActivity;

    public bool Errored() => _error.Errored();

    public Error Error() => _error.Error();
}
