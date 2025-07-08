using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests.Features;

public sealed class StartParserTests : IClassFixture<CacheAdapterParsersFixture>
{
    private readonly CacheAdapterParsersFixture _fixture;

    public StartParserTests(CacheAdapterParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Start_Parser_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        string expectedState = ParserState.Working();
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await _fixture
            .Toolkit()
            .AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await _fixture
            .Toolkit()
            .ChangeLinkActivitySuccessAsync(
                parser.Identification().ReadId(),
                link.Identification().ReadId(),
                true
            );
        await _fixture.Toolkit().EnableParserSuccessAsync(parser);
        await _fixture.Toolkit().StartParserSuccessAsync(parser.Identification().ReadId());
        MaybeBag<IParser> fromCache = await _fixture
            .CachedSource()
            .Get(new ParserCacheKey(parser.Identification().ReadId()));
        Assert.True(new CompareParserState(fromCache.Take(), expectedState));
    }

    [Fact]
    private async Task Start_Working_Parser_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        string workingState = ParserState.Working();
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await _fixture
            .Toolkit()
            .AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await _fixture
            .Toolkit()
            .ChangeLinkActivitySuccessAsync(
                parser.Identification().ReadId(),
                link.Identification().ReadId(),
                true
            );
        await _fixture.Toolkit().EnableParserSuccessAsync(parser);
        await _fixture.Toolkit().UpdateParserAsyncSuccess(parser, workingState);
        await _fixture.Toolkit().StartParserFailureAsync(parser.Identification().ReadId());
    }

    [Fact]
    private async Task Start_Parser_With_All_Inactive_Links_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture
            .Toolkit()
            .AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await _fixture.Toolkit().EnableParserSuccessAsync(parser);
        await _fixture.Toolkit().StartParserFailureAsync(parser.Identification().ReadId());
    }

    [Fact]
    private async Task Start_Parser_Without_Links_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture.Toolkit().EnableParserSuccessAsync(parser);
        await _fixture.Toolkit().StartParserFailureAsync(parser.Identification().ReadId());
    }
}
