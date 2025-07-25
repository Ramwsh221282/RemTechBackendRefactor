using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class StartParserTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public StartParserTests(ParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private void Start_Parser_Success()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test parser", "Техника", "Test");
        IParserLink link = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser, "Test Link", "Test Url");
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).ChangeLinkActivitySuccess(
            parser,
            link,
            true
        );
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).EnableParserSuccess(parser);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).StartParserSuccess(parser);
    }

    [Fact]
    private void Start_Working_Parser_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test parser", "Техника", "Test");
        IParserLink link = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser, "Test Link", "Test Url");
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).ChangeLinkActivitySuccess(
            parser,
            link,
            true
        );
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).EnableParserSuccess(parser);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).StartParserSuccess(parser);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).StartParserFailure(parser);
    }

    [Fact]
    private void Start_Parser_With_All_Inactive_Links_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test parser", "Техника", "Test");
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AddLinkSuccess(
            parser,
            "Test Link",
            "Test Url"
        );
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).EnableParserSuccess(parser);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).StartParserFailure(parser);
    }

    [Fact]
    private void Start_Parser_Without_Links_Failure()
    {
        var parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test parser", "Техника", "Test");
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).EnableParserSuccess(parser);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).StartParserFailure(parser);
    }
}
