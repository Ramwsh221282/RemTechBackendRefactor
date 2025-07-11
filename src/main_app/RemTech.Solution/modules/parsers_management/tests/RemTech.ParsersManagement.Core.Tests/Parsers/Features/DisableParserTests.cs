using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class DisableParserTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public DisableParserTests(ParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private void Disable_Parser_Success()
    {
        IParsers parsers = _fixture.Parsers();
        string expectedState = ParserState.Disabled();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser();
        IParser waiting = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, ParserState.Waiting());
        IParser disabled = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).DisableParserSuccess(waiting);
        Assert.Equal(expectedState, disabled.WorkState().Read().StringValue());
    }

    [Fact]
    private void Disable_Disabled_Parser_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser();
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).DisableParserFailure(parser);
    }

    [Fact]
    private async Task Disable_Parser_Success_Async()
    {
        IParsers parsers = _fixture.Parsers();
        string waitingState = ParserState.Waiting();
        string expectedState = ParserState.Disabled();
        IParser parser = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParserAsync();
        IParser waiting = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserAsyncSuccess(parser, waitingState);
        IParser disabled = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).DisableParserSuccessAsync(waiting.Identification().ReadId());
        Assert.Equal(expectedState, disabled.WorkState().Read().StringValue());
    }

    [Fact]
    private async Task Disable_Disabled_Parser_Failure_Async()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParserAsync();
        await new ParserTestingToolkit(_fixture.AccessLogger(), parsers).DisableParserFailureAsync(
            parser.Identification().ReadId()
        );
    }
}
