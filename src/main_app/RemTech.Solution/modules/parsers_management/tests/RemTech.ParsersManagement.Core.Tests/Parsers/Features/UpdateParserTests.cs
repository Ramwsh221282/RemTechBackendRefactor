using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

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
        IParsers parsers = _fixture.Parsers();
        string expectedState = ParserState.Waiting();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser();
        IParser updated = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, expectedState);
        Assert.Equal(expectedState, updated.WorkState().Read().StringValue());
    }

    [Fact]
    private void Update_Parser_Wait_Days_Success()
    {
        IParsers parsers = _fixture.Parsers();
        int expectedWaitDays = 2;
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser();
        IParser updated = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, waitDays: expectedWaitDays);
        Assert.Equal(expectedWaitDays, updated.WorkSchedule().WaitDays().Read());
    }

    [Fact]
    private void Update_Parser_Wait_Days_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        int invalidWaitDays = 10;
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser();
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).UpdateParserFailure(
            parser,
            waitDays: invalidWaitDays
        );
    }

    [Fact]
    private void Update_Parser_State_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        string invalidState = "Random text";
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser();
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).UpdateParserFailure(
            parser,
            invalidState
        );
    }

    [Fact]
    private void Update_Working_Parser_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        int waitDays = 10;
        string workingState = ParserState.Working();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser();
        IParser working = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, workingState);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).UpdateParserFailure(
            working,
            waitDays: waitDays
        );
    }

    [Fact]
    private async Task Update_Parser_Wait_Days_Async_Success()
    {
        IParsers parsers = _fixture.Parsers();
        int expectedWaitDays = 4;
        IParser created = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParserAsync();
        IParser updated = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserAsyncSuccess(created, waitDays: expectedWaitDays);
        Assert.Equal(expectedWaitDays, updated.WorkSchedule().WaitDays().Read());
    }

    [Fact]
    private async Task Update_Parser_State_Async_Success()
    {
        IParsers parsers = _fixture.Parsers();
        string expectedState = ParserState.Waiting();
        IParser created = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParserAsync();
        IParser updated = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserAsyncSuccess(created, expectedState);
        Assert.Equal(expectedState, updated.WorkState().Read().StringValue());
    }

    [Fact]
    private async Task Update_Parser_Async_State_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        string invalidState = "Random text";
        IParser created = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParserAsync();
        await new ParserTestingToolkit(_fixture.AccessLogger(), parsers).UpdateParserAsyncFailure(
            created,
            invalidState
        );
    }

    [Fact]
    private async Task Update_Parser_Wait_Days_Async_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        int invalidWaitDays = 10;
        IParser created = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParserAsync();
        await new ParserTestingToolkit(_fixture.AccessLogger(), parsers).UpdateParserAsyncFailure(
            created,
            waitDays: invalidWaitDays
        );
    }

    [Fact]
    private async Task Update_Parser_Working_Async_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        string expectedState = ParserState.Working();
        IParser created = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParserAsync();
        IParser working = await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserAsyncSuccess(created, expectedState);
        await new ParserTestingToolkit(_fixture.AccessLogger(), parsers).UpdateParserAsyncFailure(
            working,
            waitDays: 4
        );
    }
}
