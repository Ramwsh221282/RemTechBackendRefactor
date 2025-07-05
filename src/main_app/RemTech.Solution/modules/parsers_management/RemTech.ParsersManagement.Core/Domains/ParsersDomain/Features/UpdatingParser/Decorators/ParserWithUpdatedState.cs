using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Decorators;

public sealed class ParserWithUpdatedState : IUpdatedParser
{
    private readonly IParser _parser;

    public ParserWithUpdatedState(IParser parser) => _parser = parser;

    public Status<IParser> Updated(UpdateParser update)
    {
        if (!update.AnyState())
            return _parser.Success();
        Status updating = _parser.ChangeState(update.TakeState());
        return updating.IsFailure ? updating.Error : _parser.Success();
    }
}
