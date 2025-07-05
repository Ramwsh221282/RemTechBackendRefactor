using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser;

public sealed class StopParser
{
    private readonly IParser _parser;

    public StopParser(IParser parser)
    {
        _parser = parser;
    }

    public IParser WhomStop() => _parser;
}