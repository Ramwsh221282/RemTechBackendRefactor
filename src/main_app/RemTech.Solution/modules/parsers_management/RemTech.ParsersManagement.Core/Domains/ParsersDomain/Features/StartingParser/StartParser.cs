using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser;

public sealed class StartParser
{
    private readonly IParser _parser;

    public StartParser(IParser parser) => _parser = parser;

    public IParser TakeStarter() => _parser;
}
