using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkActivities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests.Features;

public sealed class ChangeLinkActivityTests : IClassFixture<CacheAdapterParsersFixture>
{
    private readonly CacheAdapterParsersFixture _fixture;

    public ChangeLinkActivityTests(CacheAdapterParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Change_Link_Activity_Async_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        bool nextActivity = true;
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await _fixture
            .Toolkit()
            .AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        IParserLink changed = await _fixture
            .Toolkit()
            .ChangeLinkActivitySuccessAsync(
                parser.Identification().ReadId(),
                link.Identification().ReadId(),
                nextActivity
            );
        MaybeBag<IParser> fromCache = await _fixture
            .CachedSource()
            .Get(new ParserCacheKey(parser.Identification().ReadId()));
        Assert.True(fromCache.Any());
        IParser cached = fromCache.Take();
        LinkFromParserBag linkFrombag = cached
            .OwnedLinks()
            .FindConcrete(l => new CompareLinkIdentityById(l, changed));
        Assert.True(linkFrombag.Any());
        IParserLink bagged = linkFrombag.Link();
        Assert.True(new CompareLinkActivity(bagged, nextActivity));
    }

    [Fact]
    private async Task Change_Link_Activity_Async_Same_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        bool nextActivity = false;
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await _fixture
            .Toolkit()
            .AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await _fixture
            .Toolkit()
            .ChangeLinkActivityFailureAsync(
                parser.Identification().ReadId(),
                link.Identification().ReadId(),
                nextActivity
            );
    }

    [Fact]
    private async Task Change_Link_Activity_WorkingParser_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        string workingState = ParserState.Working();
        bool nextActivity = true;
        IParser parser = await _fixture
            .Toolkit()
            .AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await _fixture
            .Toolkit()
            .AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        IParser working = await _fixture
            .Toolkit()
            .UpdateParserAsyncSuccess(parser, state: workingState);
        await _fixture
            .Toolkit()
            .ChangeLinkActivityFailureAsync(
                working.Identification().ReadId(),
                link.Identification().ReadId(),
                nextActivity
            );
    }
}
