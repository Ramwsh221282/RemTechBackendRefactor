using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class EnableParserTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public EnableParserTests(ParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private void Enable_Parser_Success()
    {
        IParsers parsers = _fixture.Parsers();
        string expectedState = ParserState.Waiting();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser();
        IParser enabled = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).EnableParserSuccess(parser);
        Assert.Equal(expectedState, enabled.WorkState().Read().StringValue());
    }

    [Fact]
    private void Enable_Enabled_Parser_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser();
        IParser enabled = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).EnableParserSuccess(parser);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).EnableParserFailure(enabled);
    }

    [Fact]
    private void Enable_Working_Parser_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser();
        string workingState = ParserState.Working();
        IParser working = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, workingState);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).EnableParserFailure(working);
    }

    [Fact]
    private async Task Enable_Parser_Async_Success()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParserAsync();
        string expectedState = ParserState.Waiting();
        IParser enabled = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).EnableParserSuccessAsync(parser);
        Assert.Equal(expectedState, enabled.WorkState().Read().StringValue());
    }

    [Fact]
    private async Task Enable_Enabled_Parser_Async_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParserAsync();
        IParser enabled = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).EnableParserSuccessAsync(parser);
        await new ParserTestingToolkit(_fixture.AccessLogger(), parsers).EnableParserFailureAsync(
            enabled
        );
    }

    [Fact]
    private async Task Enable_Working_Parser_Async_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        string workingState = ParserState.Working();
        IParser parser = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParserAsync();
        IParser working = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserAsyncSuccess(parser, workingState);
        await new ParserTestingToolkit(_fixture.AccessLogger(), parsers).EnableParserFailureAsync(
            working
        );
    }
}
