using RemTech.ParsersManagement.Core.Common.Errors;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed;

public sealed class IncreaseProcessed : IMaybeError
{
    private readonly IParser _parser;
    private readonly ErrorBag _error;
    private readonly Status<NotEmptyGuid> _linkId;

    public IncreaseProcessed(IParser parser, IParserLink link)
        : this(parser, link.Identification().ReadId()) { }

    public IncreaseProcessed(IParser parser, Guid? linkId)
    {
        _parser = parser;
        _linkId = NotEmptyGuid.New(linkId);
        _error = ErrorBag.New(_linkId);
    }

    public IParser TakeOwner() => _parser;

    public NotEmptyGuid TakeIncreaserId() => _linkId;

    public bool Errored() => _error.Errored();

    public Error Error() => _error.Error();
}
