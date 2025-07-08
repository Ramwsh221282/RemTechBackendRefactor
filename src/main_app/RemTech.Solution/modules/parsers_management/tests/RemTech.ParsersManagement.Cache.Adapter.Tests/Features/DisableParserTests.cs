using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests.Features;

public sealed class DisableParserTests : IClassFixture<CacheAdapterParsersFixture>
{
    private readonly CacheAdapterParsersFixture _fixture;

    public DisableParserTests(CacheAdapterParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Disable_Parser_Async_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test Domain";
        string expectedState = ParserState.Disabled();
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParser enabled = await _fixture
            .Toolkit()
            .UpdateParserAsyncSuccess(parser, state: ParserState.Waiting());
        IParser disabled = await _fixture
            .Toolkit()
            .DisableParserSuccessAsync(enabled.Identification().ReadId());
        MaybeBag<IParser> fromCache = await _fixture
            .CachedSource()
            .Get(new ParserCacheKey(disabled.Identification().ReadId()));
        Assert.True(fromCache.Any());
        Assert.True(new CompareParserState(fromCache.Take(), expectedState));
    }

    [Fact]
    private async Task Disable_Disabled_Parser_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test Domain";
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture.Toolkit().DisableParserFailureAsync(parser.Identification().ReadId());
    }
}
