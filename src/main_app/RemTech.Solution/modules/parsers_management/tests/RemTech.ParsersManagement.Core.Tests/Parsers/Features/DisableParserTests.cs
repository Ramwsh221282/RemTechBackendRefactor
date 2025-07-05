using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class DisableParserTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public DisableParserTests(ParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private void Disable_Parser_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string expectedState = ParserState.Disabled();
        IParser parser = toolkit.CreateInitialParser();
        IParser waiting = toolkit.UpdateParserSuccess(parser, ParserState.Waiting());
        IParser disabled = toolkit.DisableParserSuccess(waiting);
        Assert.Equal(expectedState, disabled.WorkState().Read().StringValue());
    }

    [Fact]
    private void Disable_Disabled_Parser_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser();
        toolkit.DisableParserFailure(parser);
    }

    [Fact]
    private async Task Disable_Parser_Success_Async()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string waitingState = ParserState.Waiting();
        string expectedState = ParserState.Disabled();
        IParser parser = await toolkit.CreateInitialParserAsync();
        IParser waiting = await toolkit.UpdateParserAsyncSuccess(parser, waitingState);
        IParser disabled = await toolkit.DisableParserSuccessAsync(
            waiting.Identification().ReadId()
        );
        Assert.Equal(expectedState, disabled.WorkState().Read().StringValue());
    }

    [Fact]
    private async Task Disable_Disabled_Parser_Failure_Async()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = await toolkit.CreateInitialParserAsync();
        await toolkit.DisableParserFailureAsync(parser.Identification().ReadId());
    }
}
