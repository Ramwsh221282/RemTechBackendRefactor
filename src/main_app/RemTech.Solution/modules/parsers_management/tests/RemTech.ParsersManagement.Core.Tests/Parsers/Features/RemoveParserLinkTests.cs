using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class RemoveParserLinkTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public RemoveParserLinkTests(ParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private void Remove_Link_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        toolkit.RemoveLinkSuccess(parser, link);
        ParserLinksBag links = parser.OwnedLinks();
        Assert.Equal(0, links.Amount());
    }

    [Fact]
    private void Remove_Link_Not_Found()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        toolkit.RemoveLinkFailure(parser, Guid.NewGuid());
    }

    [Fact]
    private void Remove_Link_Wrong_Domain()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser1 = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        IParser parser2 = toolkit.CreateInitialParser("Other Parser", "Техника", "Other");
        toolkit.AddLinkSuccess(parser1, "Test Link", "Test Url");
        IParserLink link = toolkit.AddLinkSuccess(parser2, "Other Link", "Other Url");
        toolkit.RemoveLinkFailure(parser1, link);
    }

    [Fact]
    private void Remove_Link_Working_Parser()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = toolkit.CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = toolkit.AddLinkSuccess(parser, "Test Link", "Test Url");
        string workingState = ParserState.Working();
        IParser working = toolkit.UpdateParserSuccess(parser, workingState);
        toolkit.RemoveLinkFailure(working, link);
    }
}
