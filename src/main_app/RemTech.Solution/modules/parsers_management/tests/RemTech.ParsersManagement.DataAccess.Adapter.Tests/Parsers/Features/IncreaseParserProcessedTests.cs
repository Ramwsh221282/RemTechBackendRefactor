using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers.Features;

public sealed class IncreaseParserProcessedTests : IClassFixture<DataAccessParsersFixture>
{
    private readonly DataAccessParsersFixture _fixture;

    public IncreaseParserProcessedTests(DataAccessParsersFixture fixture)
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
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).ChangeLinkActivitySuccessAsync(
            parser.Identification().ReadId(),
            link.Identification().ReadId(),
            activeLink
        );
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).EnableParserSuccessAsync(parser);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).StartParserSuccessAsync(parser.Identification().ReadId());
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncIncreaseProcessedSuccess(
            parser.Identification().ReadId(),
            link.Identification().ReadId()
        );
        await using ParsersSource source = _fixture.ParsersSource();
        Status<IParser> fromDb = await source.Find(parser.Identification().ReadName());
        Assert.True(fromDb.IsSuccess);
        LinkFromParserBag linkFromBag = fromDb
            .Value.OwnedLinks()
            .FindConcrete(l => new CompareLinkIdentityById(l, link));
        Assert.True(linkFromBag.Any());
        int parserProcessed = fromDb.Value.WorkedStatistics().ProcessedAmount().Read();
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
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).ChangeLinkActivitySuccessAsync(
            parser.Identification().ReadId(),
            link.Identification().ReadId(),
            activeLink
        );
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncIncreaseProcessedFailure(
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
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).ChangeLinkActivitySuccessAsync(
            parser.Identification().ReadId(),
            link.Identification().ReadId(),
            activeLink
        );
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).EnableParserSuccessAsync(parser);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).StartParserSuccessAsync(parser.Identification().ReadId());
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncIncreaseProcessedFailure(parser.Identification().ReadId(), Guid.NewGuid());
    }
}
