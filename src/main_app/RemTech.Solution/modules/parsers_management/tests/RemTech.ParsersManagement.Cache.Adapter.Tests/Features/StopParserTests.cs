using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests.Features;

public sealed class StopParserTests : IClassFixture<CacheAdapterParsersFixture>
{
    private readonly CacheAdapterParsersFixture _fixture;

    public StopParserTests(CacheAdapterParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Stop_Parser_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        string expectedState = ParserState.Waiting();
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
        await _fixture.Toolkit().AsyncStoppedParserSuccess(parser.Identification().ReadId());
        MaybeBag<IParser> fromDb = await _fixture
            .CachedSource()
            .Get(new ParserCacheKey(parser.Identification().ReadId()));
        Assert.True(fromDb.Any());
        Assert.True(new CompareParserState(fromDb.Take(), expectedState));
    }

    [Fact]
    private async Task Stop_Parser_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture.Toolkit().AsyncStoppedParserFailure(parser.Identification().ReadId());
    }
}
