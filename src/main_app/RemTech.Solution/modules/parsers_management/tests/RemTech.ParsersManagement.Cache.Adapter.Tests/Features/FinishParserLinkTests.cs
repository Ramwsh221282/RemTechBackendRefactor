using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests.Features;

public sealed class FinishParserLinkTests : IClassFixture<CacheAdapterParsersFixture>
{
    private readonly CacheAdapterParsersFixture _fixture;

    public FinishParserLinkTests(CacheAdapterParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Finish_Parser_Link_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test";
        long elapsed = 60;
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
        await _fixture
            .Toolkit()
            .UpdateParserAsyncSuccess(parser.Identification().ReadId(), state: workingState);
        await _fixture
            .Toolkit()
            .AsyncFinishLinkSuccess(
                parser.Identification().ReadId(),
                link.Identification().ReadId(),
                elapsed
            );
        MaybeBag<IParser> fromCache = await _fixture
            .CachedSource()
            .Get(new ParserCacheKey(parser.Identification().ReadId()));
        Assert.True(fromCache.Any());
        LinkFromParserBag linkFromBag = fromCache
            .Take()
            .OwnedLinks()
            .FindConcrete(l => new CompareLinkIdentityById(l, link));
        Assert.True(linkFromBag.Any());
        IParserLink bagged = linkFromBag.Link();
        int minutes = bagged.WorkedStatistic().WorkedTime().Minutes().Read();
        int hours = bagged.WorkedStatistic().WorkedTime().Hours().Read();
        int seconds = bagged.WorkedStatistic().WorkedTime().Seconds().Read();
        long totalElapsed = bagged.WorkedStatistic().WorkedTime().Total();
        Assert.Equal(0, hours);
        Assert.Equal(1, minutes);
        Assert.Equal(0, seconds);
        Assert.Equal(elapsed, totalElapsed);
    }

    [Fact]
    private async Task Finish_Parser_Link_Parser_Not_Working_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test";
        long elapsed = 60;
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
        await _fixture
            .Toolkit()
            .AsyncFinishLinkFailure(
                parser.Identification().ReadId(),
                link.Identification().ReadId(),
                elapsed
            );
    }

    [Fact]
    private async Task Finish_Parser_Link_Not_Activated_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test";
        long elapsed = 60;
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await _fixture
            .Toolkit()
            .AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await _fixture
            .Toolkit()
            .AsyncFinishLinkFailure(
                parser.Identification().ReadId(),
                link.Identification().ReadId(),
                elapsed
            );
    }

    [Fact]
    private async Task Finish_Parser_Link_Not_Existed_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test";
        long elapsed = 60;
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await _fixture
            .Toolkit()
            .AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await _fixture
            .Toolkit()
            .AsyncFinishLinkFailure(parser.Identification().ReadId(), Guid.NewGuid(), elapsed);
    }

    [Fact]
    private async Task Finish_Parser_Link_Invalid_Elapsed_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test";
        long elapsed = -60;
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
        await _fixture
            .Toolkit()
            .UpdateParserAsyncSuccess(parser.Identification().ReadId(), state: workingState);
        await _fixture
            .Toolkit()
            .AsyncFinishLinkFailure(
                parser.Identification().ReadId(),
                link.Identification().ReadId(),
                elapsed
            );
    }
}
