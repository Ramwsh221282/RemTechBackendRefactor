using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers.Features;

public sealed class AddParserLinkTests : IClassFixture<DataAccessParsersFixture>
{
    private readonly DataAccessParsersFixture _fixture;

    public AddParserLinkTests(DataAccessParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private async Task Add_Parser_Link_Async_Success()
    {
        string parserDomain = "Test";
        string parserName = "Test Parser";
        string parserType = "Техника";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        IParserLink link = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await using ParsersSource source = _fixture.ParsersSource();
        Status<IParser> fromDb = await source.Find(parser.Identification().ReadId());
        Assert.True(fromDb.IsSuccess);
        ParserLinksBag bag = fromDb.Value.OwnedLinks();
        LinkFromParserBag linkFrombag = bag.FindConcrete(l => new CompareLinkIdentityById(l, link));
        Assert.True(linkFrombag.Any());
        IParserLink fromBag = linkFrombag.Link();
        Assert.True(new CompareLinkIdentityById(link, fromBag));
        Assert.True(new CompareLinkIdentityByName(link, fromBag));
        Assert.True(new CompareLinkIdentityByParserOwning(fromBag, parser));
        Assert.True(new CompareLinkIdentityByParserServiceDomain(fromBag, parser));
    }

    [Fact]
    private async Task Add_Parser_Link_Async_Name_Failure()
    {
        string parserDomain = "Test";
        string parserName = "Test Parser";
        string parserType = "Техника";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        string linkName = string.Empty;
        string linkUrl = "Test Url";
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkFailureAsync(parser.Identification().ReadId(), linkName, linkUrl);
    }

    [Fact]
    private async Task Add_Parser_Link_Async_Url_Failure()
    {
        string parserDomain = "Test";
        string parserName = "Test Parser";
        string parserType = "Техника";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        string linkName = "Test Link";
        string linkUrl = string.Empty;
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkFailureAsync(parser.Identification().ReadId(), linkName, linkUrl);
    }

    [Fact]
    private async Task Add_Parser_Link_Async_Domain_Failure()
    {
        string parserDomain = "Test";
        string parserName = "Test Parser";
        string parserType = "Техника";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        string linkName = "Test Link";
        string linkUrl = "Other Url";
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkFailureAsync(parser.Identification().ReadId(), linkName, linkUrl);
    }

    [Fact]
    private async Task Add_Parser_Link_Duplicate_Name_Failure()
    {
        string parserDomain = "Test";
        string parserName = "Test Parser";
        string parserType = "Техника";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        string duplicateName = linkName;
        string otherUrl = "Other Url";
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkFailureAsync(parser.Identification().ReadId(), duplicateName, otherUrl);
    }

    [Fact]
    private async Task Add_Parser_Link_Duplicate_Url_Failure()
    {
        string parserDomain = "Test";
        string parserName = "Test Parser";
        string parserType = "Техника";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        string otherName = "Other name";
        string duplicateUrl = linkUrl;
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkFailureAsync(parser.Identification().ReadId(), otherName, duplicateUrl);
    }

    [Fact]
    private async Task Add_Parser_Link_To_WorkingParser_Failure()
    {
        string parserDomain = "Test";
        string parserName = "Test Parser";
        string parserType = "Техника";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        string workingState = ParserState.Working();
        IParser working = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).UpdateParserAsyncSuccess(parser.Identification().ReadId(), state: workingState);

        string linkName = "Test Link";
        string linkUrl = "Test Url";
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkFailureAsync(working.Identification().ReadId(), linkName, linkUrl);
    }
}
