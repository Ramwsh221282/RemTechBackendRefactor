using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class StartParserTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public StartParserTests(ParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private void Start_Parser_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        toolkit.ChangeLinkActivitySuccess(parser, link, true);
        toolkit.EnableParserSuccess(parser);
        toolkit.StartParserSuccess(parser);
    }

    [Fact]
    private void Start_Working_Parser_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        toolkit.ChangeLinkActivitySuccess(parser, link, true);
        toolkit.EnableParserSuccess(parser);
        toolkit.StartParserSuccess(parser);
        toolkit.StartParserFailure(parser);
    }

    [Fact]
    private void Start_Parser_With_All_Inactive_Links_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test parser", "Техника", "Test");
        toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        toolkit.EnableParserSuccess(parser);
        toolkit.StartParserFailure(parser);
    }

    [Fact]
    private void Start_Parser_Without_Links_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test parser", "Техника", "Test");
        toolkit.EnableParserSuccess(parser);
        toolkit.StartParserFailure(parser);
    }
}
