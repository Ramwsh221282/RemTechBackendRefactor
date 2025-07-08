using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests.Features;

public sealed class RemoveParserLinkTests : IClassFixture<CacheAdapterParsersFixture>
{
    private readonly CacheAdapterParsersFixture _fixture;

    public RemoveParserLinkTests(CacheAdapterParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Remove_Link_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test link";
        string linkDomain = "Test";
        int expectedBagAmount = 0;
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await _fixture
            .Toolkit()
            .AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkDomain);
        await _fixture
            .Toolkit()
            .AsyncRemoveLinkSuccess(
                parser.Identification().ReadId(),
                link.Identification().ReadId()
            );
        MaybeBag<IParser> fromCache = await _fixture
            .CachedSource()
            .Get(new ParserCacheKey(parser.Identification().ReadId()));
        Assert.True(fromCache.Any());
        ParserLinksBag linksBag = fromCache.Take().OwnedLinks();
        Assert.Equal(expectedBagAmount, linksBag.Amount());
    }

    [Fact]
    private async Task Remove_Link_Not_Found()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture
            .Toolkit()
            .AsyncRemoveLinkFailure(parser.Identification().ReadId(), Guid.NewGuid());
    }

    [Fact]
    private async Task Remove_Link_Working_Parser()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test link";
        string linkDomain = "Test";
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await _fixture
            .Toolkit()
            .AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkDomain);
        await _fixture
            .Toolkit()
            .UpdateParserAsyncSuccess(
                parser.Identification().ReadId(),
                state: ParserState.Working()
            );
        await _fixture
            .Toolkit()
            .AsyncRemoveLinkFailure(
                parser.Identification().ReadId(),
                link.Identification().ReadId()
            );
    }
}
