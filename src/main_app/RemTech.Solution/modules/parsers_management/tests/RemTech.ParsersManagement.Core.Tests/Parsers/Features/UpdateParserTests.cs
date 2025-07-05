using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class UpdateParserTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public UpdateParserTests(ParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private void Update_Parser_State_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string expectedState = ParserState.Waiting();
        IParser parser = toolkit.CreateInitialParser();
        IParser updated = toolkit.UpdateParserSuccess(parser, expectedState);
        Assert.Equal(expectedState, updated.WorkState().Read().StringValue());
    }

    [Fact]
    private void Update_Parser_Wait_Days_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        int expectedWaitDays = 2;
        IParser parser = toolkit.CreateInitialParser();
        IParser updated = toolkit.UpdateParserSuccess(parser, waitDays: expectedWaitDays);
        Assert.Equal(expectedWaitDays, updated.WorkSchedule().WaitDays().Read());
    }

    [Fact]
    private void Update_Parser_Wait_Days_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        int invalidWaitDays = 10;
        IParser parser = toolkit.CreateInitialParser();
        toolkit.UpdateParserFailure(parser, waitDays: invalidWaitDays);
    }

    [Fact]
    private void Update_Parser_State_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string invalidState = "Random text";
        IParser parser = toolkit.CreateInitialParser();
        toolkit.UpdateParserFailure(parser, invalidState);
    }

    [Fact]
    private void Update_Working_Parser_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        int waitDays = 10;
        string workingState = ParserState.Working();
        IParser parser = toolkit.CreateInitialParser();
        IParser working = toolkit.UpdateParserSuccess(parser, workingState);
        toolkit.UpdateParserFailure(working, waitDays: waitDays);
    }

    [Fact]
    private async Task Update_Parser_Wait_Days_Async_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        int expectedWaitDays = 4;
        IParser created = await toolkit.CreateInitialParserAsync();
        IParser updated = await toolkit.UpdateParserAsyncSuccess(
            created,
            waitDays: expectedWaitDays
        );
        Assert.Equal(expectedWaitDays, updated.WorkSchedule().WaitDays().Read());
    }

    [Fact]
    private async Task Update_Parser_State_Async_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string expectedState = ParserState.Waiting();
        IParser created = await toolkit.CreateInitialParserAsync();
        IParser updated = await toolkit.UpdateParserAsyncSuccess(created, expectedState);
        Assert.Equal(expectedState, updated.WorkState().Read().StringValue());
    }

    [Fact]
    private async Task Update_Parser_Async_State_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string invalidState = "Random text";
        IParser created = await toolkit.CreateInitialParserAsync();
        await toolkit.UpdateParserAsyncFailure(created, invalidState);
    }

    [Fact]
    private async Task Update_Parser_Wait_Days_Async_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        int invalidWaitDays = 10;
        IParser created = await toolkit.CreateInitialParserAsync();
        await toolkit.UpdateParserAsyncFailure(created, waitDays: invalidWaitDays);
    }

    [Fact]
    private async Task Update_Parser_Working_Async_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string expectedState = ParserState.Working();
        IParser created = await toolkit.CreateInitialParserAsync();
        IParser working = await toolkit.UpdateParserAsyncSuccess(created, expectedState);
        await toolkit.UpdateParserAsyncFailure(working, waitDays: 4);
    }
}
