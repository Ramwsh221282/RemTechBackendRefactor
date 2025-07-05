using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class ChangeLinkActivityTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public ChangeLinkActivityTests(ParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private void Change_Link_Activity_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        bool currentActivity = link.Activity();
        Assert.False(currentActivity);
        IParserLink active = toolkit.ChangeLinkActivitySuccess(parser, link, true);
        bool nextActivity = active.Activity();
        Assert.True(nextActivity);
        Assert.NotEqual(currentActivity, nextActivity);
    }

    [Fact]
    private void Change_Link_Activity_Same_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        toolkit.ChangeLinkActivitySuccess(parser, link, true);
        toolkit.ChangeLinkActivityFailure(parser, link, true);
    }

    [Fact]
    private void Change_Link_Activity_WorkingParser_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        string workingState = ParserState.Working();
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        IParser working = toolkit.UpdateParserSuccess(parser, workingState);
        toolkit.ChangeLinkActivityFailure(working, link, true);
    }
}
