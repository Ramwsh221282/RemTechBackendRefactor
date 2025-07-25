using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers.Features;

public class RemoveParserLinkTests : IClassFixture<DataAccessParsersFixture>
{
    private readonly DataAccessParsersFixture _fixture;

    public RemoveParserLinkTests(DataAccessParsersFixture fixture)
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
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncRemoveLinkSuccess(parser.Identification().ReadId(), link.Identification().ReadId());
        await using IParsers source = _fixture.Parsers();
        Status<IParser> fromDb = await source.Find(parser.Identification().ReadId());
        ParserLinksBag linksBag = fromDb.Value.OwnedLinks();
        Assert.True(fromDb.IsSuccess);
        Assert.Equal(expectedBagAmount, linksBag.Amount());
    }

    [Fact]
    private async Task Remove_Link_Not_Found()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncRemoveLinkFailure(parser.Identification().ReadId(), Guid.NewGuid());
    }

    [Fact]
    private async Task Remove_Link_Working_Parser()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test link";
        string linkDomain = "Test";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).UpdateParserAsyncSuccess(parser.Identification().ReadId(), state: ParserState.Working());
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncRemoveLinkFailure(parser.Identification().ReadId(), link.Identification().ReadId());
    }
}
