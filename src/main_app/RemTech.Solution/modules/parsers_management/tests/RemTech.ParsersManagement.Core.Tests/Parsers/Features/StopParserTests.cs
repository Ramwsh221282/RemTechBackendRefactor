using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class StopParserTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public StopParserTests(ParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private void Stop_Parser_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        toolkit.ChangeLinkActivitySuccess(parser, link, true);
        toolkit.EnableParserSuccess(parser);
        toolkit.StartParserSuccess(parser);
        toolkit.StoppedParserSuccess(parser);
    }

    [Fact]
    private void Stop_Parser_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test parser", "Техника", "Test");
        toolkit.StoppedParserFailure(parser);
    }
}
