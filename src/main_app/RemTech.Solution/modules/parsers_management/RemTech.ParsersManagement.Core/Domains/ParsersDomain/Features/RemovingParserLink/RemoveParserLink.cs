using RemTech.Core.Shared.Functional;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink;

public sealed class RemoveParserLink : IMaybeError
{
    private readonly IParser _parser;
    private readonly ErrorBag _error;
    private readonly Status<NotEmptyGuid> _linkId;

    public RemoveParserLink(IParser parser, Guid? linkId)
    {
        _parser = parser;
        _linkId = NotEmptyGuid.New(linkId);
        _error = ErrorBag.New(_linkId);
    }

    public RemoveParserLink(IParser parser, NotEmptyGuid linkId)
    {
        _parser = parser;
        _linkId = linkId;
        _error = ErrorBag.New();
    }

    public RemoveParserLink(IParser parser, IParserLink link)
    {
        _parser = parser;
        _linkId = link.Identification().ReadId();
        _error = ErrorBag.New();
    }

    public NotEmptyGuid RemovingId() => _linkId.Value;

    public bool Errored() => _error.Errored();

    public Error Error() => _error.Error();

    public IParser TakeOwner() => _parser;
}
