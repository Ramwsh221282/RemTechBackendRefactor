using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class IncreaseParserProcessedTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public IncreaseParserProcessedTests(ParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private void Increase_Statistics_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        IParserLink active = toolkit.ChangeLinkActivitySuccess(parser, link, true);
        string workingState = ParserState.Working();
        IParser working = toolkit.UpdateParserSuccess(parser, workingState);
        ParserStatisticsIncreasement increasement = toolkit.IncreaseProcessedSuccess(
            working,
            active
        );
        Assert.Equal(1, increasement.CurrentProcessed());
        Assert.Equal(1, increasement.LinkIncreasement().CurrentProcessed());
    }

    [Fact]
    private void Increase_Statistics_When_Not_Working_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        IParserLink active = toolkit.ChangeLinkActivitySuccess(parser, link, true);
        toolkit.IncreaseProcessedFailure(parser, active);
    }

    [Fact]
    private void Increase_Statistics_Link_Not_Found_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        string workingState = ParserState.Working();
        IParser working = toolkit.UpdateParserSuccess(parser, workingState);
        toolkit.IncreaseProcessedFailure(working, Guid.NewGuid());
    }

    [Fact]
    private void Increase_Statistics_Link_Not_Active_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        string workingState = ParserState.Working();
        IParser working = toolkit.UpdateParserSuccess(parser, workingState);
        toolkit.IncreaseProcessedFailure(working, link);
    }
}
