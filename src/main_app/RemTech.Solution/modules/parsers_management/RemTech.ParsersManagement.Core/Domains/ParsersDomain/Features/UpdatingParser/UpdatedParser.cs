using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser;

public sealed class UpdatedParser : IUpdatedParser
{
    public Status<IParser> Updated(UpdateParser update)
    {
        IParser parser = update.Take();
        return new SomehowUpdatedParser(
            parser,
            new ParserWithUpdatedState(parser),
            new ParserWithUpdatedSchedule(parser)
        ).Updated(update);
    }
}
