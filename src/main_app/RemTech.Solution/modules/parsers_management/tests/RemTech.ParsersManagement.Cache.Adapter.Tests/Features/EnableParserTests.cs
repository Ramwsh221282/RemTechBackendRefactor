using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests.Features;

public sealed class EnableParserTests : IClassFixture<CacheAdapterParsersFixture>
{
    private readonly CacheAdapterParsersFixture _fixture;

    public EnableParserTests(CacheAdapterParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Enable_Parser_Async_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string expectedState = ParserState.Waiting();
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture.Toolkit().EnableParserSuccessAsync(parser);
        MaybeBag<IParser> fromCache = await _fixture
            .CachedSource()
            .Get(new ParserCacheKey(parser.Identification().ReadId()));
        Assert.True(fromCache.Any());
        Assert.True(new CompareParserState(fromCache.Take(), expectedState));
    }

    [Fact]
    private async Task Enable_Enabled_Parser_Async_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture.Toolkit().EnableParserSuccessAsync(parser);
        await _fixture.Toolkit().EnableParserFailureAsync(parser);
    }

    [Fact]
    private async Task Enable_Working_Parser_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture.Toolkit().UpdateParserAsyncSuccess(parser, state: ParserState.Working());
        await _fixture.Toolkit().EnableParserFailureAsync(parser);
    }
}
