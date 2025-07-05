using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;

namespace RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

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
