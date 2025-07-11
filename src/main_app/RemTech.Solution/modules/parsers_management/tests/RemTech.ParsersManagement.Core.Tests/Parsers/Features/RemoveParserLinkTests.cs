using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
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
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser, "Test Link", "Test Url");
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).RemoveLinkSuccess(parser, link);
        ParserLinksBag links = parser.OwnedLinks();
        Assert.Equal(0, links.Amount());
    }

    [Fact]
    private void Remove_Link_Not_Found()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AddLinkSuccess(
            parser,
            "Test Link",
            "Test Url"
        );
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).RemoveLinkFailure(
            parser,
            Guid.NewGuid()
        );
    }

    [Fact]
    private void Remove_Link_Wrong_Domain()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser1 = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        IParser parser2 = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Other Parser", "Техника", "Other");
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AddLinkSuccess(
            parser1,
            "Test Link",
            "Test Url"
        );
        IParserLink link = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser2, "Other Link", "Other Url");
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).RemoveLinkFailure(parser1, link);
    }

    [Fact]
    private void Remove_Link_Working_Parser()
    {
        IParsers parsers = _fixture.Parsers();
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test Parser", "Техника", "Test");
        IParserLink link = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser, "Test Link", "Test Url");
        string workingState = ParserState.Working();
        IParser working = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, workingState);
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).RemoveLinkFailure(working, link);
    }
}
