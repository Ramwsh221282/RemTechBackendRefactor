using RemTech.Logging.Adapter;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;


namespace RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

public sealed class ParsersFixture
{
    public IParsers Parsers()
    {
        MokParsers parsers = new();
        MokValidParsers valid = new(parsers);
        return valid;
    }

    public Serilog.ILogger AccessLogger() => new LoggerSource().Logger();
}
