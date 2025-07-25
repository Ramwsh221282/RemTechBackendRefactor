using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class ChangeLinkActivityTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public ChangeLinkActivityTests(ParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private void Change_Link_Activity_Success()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser, "Test Link", "Test Url");
        bool currentActivity = link.Activity();
        Assert.False(currentActivity);
        IParserLink active = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).ChangeLinkActivitySuccess(parser, link, true);
        bool nextActivity = active.Activity();
        Assert.True(nextActivity);
        Assert.NotEqual(currentActivity, nextActivity);
    }

    [Fact]
    private void Change_Link_Activity_Same_Failure()
    {
        var parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser, "Test Link", "Test Url");
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).ChangeLinkActivitySuccess(
            parser,
            link,
            true
        );
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).ChangeLinkActivityFailure(
            parser,
            link,
            true
        );
    }

    [Fact]
    private void Change_Link_Activity_WorkingParser_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        string workingState = ParserState.Working();
        IParserLink link = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser, "Test Link", "Test Url");
        IParser working = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, workingState);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).ChangeLinkActivityFailure(
            working,
            link,
            true
        );
    }
}
