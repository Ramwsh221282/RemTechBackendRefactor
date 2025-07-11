using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class FinishParserLinkTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public FinishParserLinkTests(ParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private void Finish_Parser_Link_Success()
    {
        IParsers parsers = _fixture.Parsers();
        long elapsed = 60;
        string workingState = ParserState.Working();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser, "Test Link", "Test Url");
        IParserLink activated = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).ChangeLinkActivitySuccess(parser, link, true);
        IParser working = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, workingState);
        IParserLink finished = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).FinishLinkSuccess(working, activated, elapsed);
        int minutes = finished.WorkedStatistic().WorkedTime().Minutes().Read();
        int hours = finished.WorkedStatistic().WorkedTime().Hours().Read();
        int seconds = finished.WorkedStatistic().WorkedTime().Seconds().Read();
        long totalElapsed = finished.WorkedStatistic().WorkedTime().Total();
        Assert.Equal(0, hours);
        Assert.Equal(1, minutes);
        Assert.Equal(0, seconds);
        Assert.Equal(elapsed, totalElapsed);
    }

    [Fact]
    private void Finish_Parser_Link_Parser_Not_Working_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        long elapsed = 60;
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser, "Test Link", "Test Url");
        IParserLink activated = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).ChangeLinkActivitySuccess(parser, link, true);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).FinishLinkFailure(
            parser,
            activated,
            elapsed
        );
    }

    [Fact]
    private void Finish_Parser_Link_Not_Activated_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        long elapsed = 60;
        string workingState = ParserState.Working();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser, "Test Link", "Test Url");
        IParser working = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, workingState);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).FinishLinkFailure(
            working,
            link,
            elapsed
        );
    }

    [Fact]
    private void Finish_Parser_Link_Not_Existed_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        long elapsed = 60;
        string workingState = ParserState.Working();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        IParser working = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, workingState);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).FinishLinkFailure(
            working,
            Guid.NewGuid(),
            elapsed
        );
    }

    [Fact]
    private void Finish_Parser_Link_Invalid_Elapsed_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        long elapsed = -60;
        string workingState = ParserState.Working();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser, "Test Link", "Test Url");
        IParserLink activated = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).ChangeLinkActivitySuccess(parser, link, true);
        IParser working = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, workingState);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).FinishLinkFailure(
            working,
            activated,
            elapsed
        );
    }
}
