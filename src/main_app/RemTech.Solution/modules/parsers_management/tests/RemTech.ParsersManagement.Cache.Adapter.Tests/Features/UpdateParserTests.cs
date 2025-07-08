using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests.Features;

public sealed class UpdateParserTests : IClassFixture<CacheAdapterParsersFixture>
{
    private readonly CacheAdapterParsersFixture _fixture;

    public UpdateParserTests(CacheAdapterParsersFixture fixture)
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
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture.Toolkit().UpdateParserAsyncSuccess(parser, waitDays: expectedWaitDays);
        MaybeBag<IParser> fromCache = await _fixture
            .CachedSource()
            .Get(parser.Identification().ReadName());
        Assert.True(fromCache.Any());
        Assert.True(new CompareParserWaitDays(fromCache.Take(), expectedWaitDays));
    }

    [Fact]
    private async Task Update_Parser_State_Async_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string expectedState = ParserState.Waiting();
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture.Toolkit().UpdateParserAsyncSuccess(parser, state: expectedState);
        MaybeBag<IParser> fromCache = await _fixture
            .CachedSource()
            .Get(parser.Identification().ReadName());
        Assert.True(fromCache.Any());
        Assert.True(new CompareParserState(fromCache.Take(), expectedState));
    }

    [Fact]
    private async Task Update_Parser_Async_State_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string invalidState = "Random text";
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture.Toolkit().UpdateParserAsyncFailure(parser, state: invalidState);
    }

    [Fact]
    private async Task Update_Parser_Wait_Days_Async_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        int invalidWaitDays = 10;
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture.Toolkit().UpdateParserAsyncFailure(parser, waitDays: invalidWaitDays);
    }

    [Fact]
    private async Task Update_Parser_Async_State_And_WaitDays_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        int invalidWaitDays = 10;
        string invalidState = "Random text";
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture
            .Toolkit()
            .UpdateParserAsyncFailure(parser, state: invalidState, waitDays: invalidWaitDays);
    }

    [Fact]
    private async Task Update_Parser_Working_Async_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string invalidState = ParserState.Working();
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture.Toolkit().UpdateParserAsyncSuccess(parser, state: invalidState);
        await _fixture.Toolkit().UpdateParserAsyncFailure(parser, state: invalidState);
    }
}
