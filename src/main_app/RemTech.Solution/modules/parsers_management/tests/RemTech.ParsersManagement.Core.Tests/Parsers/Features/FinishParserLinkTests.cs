using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

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
        ParserTestingToolkit toolkit = new(_fixture);
        long elapsed = 60;
        string workingState = ParserState.Working();
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        IParserLink activated = toolkit.ChangeLinkActivitySuccess(parser, link, true);
        IParser working = toolkit.UpdateParserSuccess(parser, workingState);
        IParserLink finished = toolkit.FinishLinkSuccess(working, activated, elapsed);
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
        ParserTestingToolkit toolkit = new(_fixture);
        long elapsed = 60;
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        IParserLink activated = toolkit.ChangeLinkActivitySuccess(parser, link, true);
        toolkit.FinishLinkFailure(parser, activated, elapsed);
    }

    [Fact]
    private void Finish_Parser_Link_Not_Activated_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        long elapsed = 60;
        string workingState = ParserState.Working();
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        IParser working = toolkit.UpdateParserSuccess(parser, workingState);
        toolkit.FinishLinkFailure(working, link, elapsed);
    }

    [Fact]
    private void Finish_Parser_Link_Not_Existed_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        long elapsed = 60;
        string workingState = ParserState.Working();
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        IParser working = toolkit.UpdateParserSuccess(parser, workingState);
        toolkit.FinishLinkFailure(working, Guid.NewGuid(), elapsed);
    }

    [Fact]
    private void Finish_Parser_Link_Invalid_Elapsed_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        long elapsed = -60;
        string workingState = ParserState.Working();
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        IParserLink activated = toolkit.ChangeLinkActivitySuccess(parser, link, true);
        IParser working = toolkit.UpdateParserSuccess(parser, workingState);
        toolkit.FinishLinkFailure(working, activated, elapsed);
    }
}
