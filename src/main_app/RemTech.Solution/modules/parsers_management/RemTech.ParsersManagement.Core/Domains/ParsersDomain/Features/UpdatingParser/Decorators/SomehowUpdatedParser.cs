using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Decorators;

public sealed class SomehowUpdatedParser : IUpdatedParser
{
    private readonly IUpdatedParser[] _updatedParsers;
    private readonly IParser _parser;

    public SomehowUpdatedParser(IParser parser, params IUpdatedParser[] updatedParsers)
    {
        _parser = parser;
        _updatedParsers = updatedParsers;
    }

    public Status<IParser> Updated(UpdateParser update)
    {
        foreach (var updatedParser in _updatedParsers)
        {
            Status updating = updatedParser.Updated(update);
            if (updating.IsFailure)
                return updating.Error;
        }

        return _parser.Success();
    }
}
