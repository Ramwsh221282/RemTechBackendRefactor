using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Tests.Parsers.Mocks;

namespace RemTech.ParsersManagement.Core.Tests.Parsers;

public sealed class ParsersFixture
{
    public ParsersSource AccessParsersSource()
    {
        MokParsers parsers = new();
        MokValidParsers valid = new(parsers);
        MokTransactionalParsers transactional = new(valid);
        return new ParsersSource(valid, transactional);
    }

    public ICustomLogger AccessLogger() => new MokLogger();
}
