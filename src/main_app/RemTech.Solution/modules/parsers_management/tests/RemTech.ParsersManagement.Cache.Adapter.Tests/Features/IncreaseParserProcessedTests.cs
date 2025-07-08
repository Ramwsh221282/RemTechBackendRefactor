using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests.Features;

public sealed class IncreaseParserProcessedTests : IClassFixture<CacheAdapterParsersFixture>
{
    private readonly CacheAdapterParsersFixture _fixture;

    public IncreaseParserProcessedTests(CacheAdapterParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Increase_Statistics_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test";
        int expectedProcessed = 1;
        bool activeLink = true;
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
                activeLink
            );
        await _fixture.Toolkit().EnableParserSuccessAsync(parser);
        await _fixture.Toolkit().StartParserSuccessAsync(parser.Identification().ReadId());
        await _fixture
            .Toolkit()
            .AsyncIncreaseProcessedSuccess(
                parser.Identification().ReadId(),
                link.Identification().ReadId()
            );
        MaybeBag<IParser> fromDb = await _fixture
            .CachedSource()
            .Get(parser.Identification().ReadName());
        Assert.True(fromDb.Any());
        LinkFromParserBag linkFromBag = fromDb
            .Take()
            .OwnedLinks()
            .FindConcrete(l => new CompareLinkIdentityById(l, link));
        Assert.True(linkFromBag.Any());
        int parserProcessed = fromDb.Take().WorkedStatistics().ProcessedAmount().Read();
        int linkProcessed = linkFromBag.Link().WorkedStatistic().ProcessedAmount().Read();
        Assert.Equal(expectedProcessed, parserProcessed);
        Assert.Equal(expectedProcessed, linkProcessed);
    }

    [Fact]
    private async Task Increase_Statistics_When_Not_Working_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test";
        bool activeLink = true;
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
                activeLink
            );
        await _fixture
            .Toolkit()
            .AsyncIncreaseProcessedFailure(
                parser.Identification().ReadId(),
                link.Identification().ReadId()
            );
    }

    [Fact]
    private async Task Increase_Statistics_Link_Not_Found_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test";
        bool activeLink = true;
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
                activeLink
            );
        await _fixture.Toolkit().EnableParserSuccessAsync(parser);
        await _fixture.Toolkit().StartParserSuccessAsync(parser.Identification().ReadId());
        await _fixture
            .Toolkit()
            .AsyncIncreaseProcessedFailure(parser.Identification().ReadId(), Guid.NewGuid());
    }
}
