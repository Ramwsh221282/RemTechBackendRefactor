using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Decorators;

public sealed class ParserWithUpdatedSchedule : IUpdatedParser
{
    private readonly IParser _parser;

    public ParserWithUpdatedSchedule(IParser parser) => _parser = parser;

    public Status<IParser> Updated(UpdateParser update)
    {
        if (!update.AnyWaitDays())
            return _parser.Success();
        Status updating = _parser.ChangeWaitDays(update.WaitDays());
        return updating.IsFailure ? updating.Error : _parser.Success();
    }
}
