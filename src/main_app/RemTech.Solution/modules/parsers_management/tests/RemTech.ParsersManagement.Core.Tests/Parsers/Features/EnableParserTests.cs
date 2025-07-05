using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

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
        ParserTestingToolkit toolkit = new(_fixture);
        string expectedState = ParserState.Waiting();
        IParser parser = toolkit.CreateInitialParser();
        IParser enabled = toolkit.EnableParserSuccess(parser);
        Assert.Equal(expectedState, enabled.WorkState().Read().StringValue());
    }

    [Fact]
    private void Enable_Enabled_Parser_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser();
        IParser enabled = toolkit.EnableParserSuccess(parser);
        toolkit.EnableParserFailure(enabled);
    }

    [Fact]
    private void Enable_Working_Parser_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser();
        string workingState = ParserState.Working();
        IParser working = toolkit.UpdateParserSuccess(parser, workingState);
        toolkit.EnableParserFailure(working);
    }

    [Fact]
    private async Task Enable_Parser_Async_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = await toolkit.CreateInitialParserAsync();
        string expectedState = ParserState.Waiting();
        IParser enabled = await toolkit.EnableParserSuccessAsync(parser);
        Assert.Equal(expectedState, enabled.WorkState().Read().StringValue());
    }

    [Fact]
    private async Task Enable_Enabled_Parser_Async_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = await toolkit.CreateInitialParserAsync();
        IParser enabled = await toolkit.EnableParserSuccessAsync(parser);
        await toolkit.EnableParserFailureAsync(enabled);
    }

    [Fact]
    private async Task Enable_Working_Parser_Async_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string workingState = ParserState.Working();
        IParser parser = await toolkit.CreateInitialParserAsync();
        IParser working = await toolkit.UpdateParserAsyncSuccess(parser, workingState);
        await toolkit.EnableParserFailureAsync(working);
    }
}
