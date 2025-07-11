using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class StopParserTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public StopParserTests(ParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private void Stop_Parser_Success()
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
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).StoppedParserSuccess(parser);
    }

    [Fact]
    private void Stop_Parser_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test parser", "Техника", "Test");
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).StoppedParserFailure(parser);
    }
}
