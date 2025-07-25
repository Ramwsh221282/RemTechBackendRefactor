using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
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
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser, "Test Link", "Test Url");
        IParserLink active = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).ChangeLinkActivitySuccess(parser, link, true);
        string workingState = ParserState.Working();
        IParser working = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, workingState);
        ParserStatisticsIncreasement increasement = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).IncreaseProcessedSuccess(working, active);
        Assert.Equal(1, increasement.CurrentProcessed());
        Assert.Equal(1, increasement.LinkIncreasement().CurrentProcessed());
    }

    [Fact]
    private void Increase_Statistics_When_Not_Working_Failure()
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
        IParserLink active = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).ChangeLinkActivitySuccess(parser, link, true);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).IncreaseProcessedFailure(
            parser,
            active
        );
    }

    [Fact]
    private void Increase_Statistics_Link_Not_Found_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        string workingState = ParserState.Working();
        IParser working = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, workingState);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).IncreaseProcessedFailure(
            working,
            Guid.NewGuid()
        );
    }

    [Fact]
    private void Increase_Statistics_Link_Not_Active_Failure()
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
        string workingState = ParserState.Working();
        IParser working = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, workingState);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).IncreaseProcessedFailure(
            working,
            link
        );
    }
}
