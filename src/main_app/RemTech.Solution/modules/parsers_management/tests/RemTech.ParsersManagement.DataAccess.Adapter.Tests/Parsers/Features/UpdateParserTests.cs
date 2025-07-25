using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers.Features;

public sealed class UpdateParserTests : IClassFixture<DataAccessParsersFixture>
{
    private readonly DataAccessParsersFixture _fixture;

    public UpdateParserTests(DataAccessParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Update_Parser_Wait_Days_Async_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        int expectedWaitDays = 4;
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).UpdateParserAsyncSuccess(parser, waitDays: expectedWaitDays);
        await using IParsers source = _fixture.Parsers();
        Status<IParser> fromDb = await source.Find(parser.Identification().ReadName());
        Assert.True(fromDb.IsSuccess);
        Assert.True(new CompareParserWaitDays(fromDb.Value, expectedWaitDays));
    }

    [Fact]
    private async Task Update_Parser_State_Async_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string expectedState = ParserState.Waiting();
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).UpdateParserAsyncSuccess(parser, state: expectedState);
        await using IParsers source = _fixture.Parsers();
        Status<IParser> fromDb = await source.Find(parser.Identification().ReadName());
        Assert.True(fromDb.IsSuccess);
        Assert.True(new CompareParserState(fromDb.Value, expectedState));
    }

    [Fact]
    private async Task Update_Parser_Async_State_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string invalidState = "Random text";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).UpdateParserAsyncFailure(parser, state: invalidState);
    }

    [Fact]
    private async Task Update_Parser_Wait_Days_Async_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        int invalidWaitDays = 10;
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).UpdateParserAsyncFailure(parser, waitDays: invalidWaitDays);
    }

    [Fact]
    private async Task Update_Parser_Async_State_And_WaitDays_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        int invalidWaitDays = 10;
        string invalidState = "Random text";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).UpdateParserAsyncFailure(parser, state: invalidState, waitDays: invalidWaitDays);
    }

    [Fact]
    private async Task Update_Parser_Working_Async_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string invalidState = ParserState.Working();
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).UpdateParserAsyncSuccess(parser, state: invalidState);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).UpdateParserAsyncFailure(parser, state: invalidState);
    }
}
