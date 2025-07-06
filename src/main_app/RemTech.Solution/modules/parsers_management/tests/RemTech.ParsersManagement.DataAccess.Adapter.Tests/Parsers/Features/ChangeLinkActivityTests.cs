using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkActivities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers.Features;

public sealed class ChangeLinkActivityTests : IClassFixture<DataAccessParsersFixture>
{
    private readonly DataAccessParsersFixture _fixture;

    public ChangeLinkActivityTests(DataAccessParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private async Task Change_Link_Activity_Async_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        bool nextActivity = true;
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        IParserLink changed = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).ChangeLinkActivitySuccessAsync(
            parser.Identification().ReadId(),
            link.Identification().ReadId(),
            nextActivity
        );
        await using ParsersSource source = _fixture.ParsersSource();
        Status<IParser> fromDb = await source.Find(parser.Identification().ReadId());
        Assert.True(fromDb.IsSuccess);
        LinkFromParserBag linkFrombag = fromDb
            .Value.OwnedLinks()
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
        ).ChangeLinkActivityFailureAsync(
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
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        IParser working = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).UpdateParserAsyncSuccess(parser, state: workingState);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).ChangeLinkActivityFailureAsync(
            working.Identification().ReadId(),
            link.Identification().ReadId(),
            nextActivity
        );
    }
}
