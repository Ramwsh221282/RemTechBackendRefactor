using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkUrls.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class AddParserLinkTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public AddParserLinkTests(ParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private void Add_Parser_Link_Success()
    {
        IParsers parsers = _fixture.Parsers();
        string domain = "Test";
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test parser", "Техника", domain);
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        IParserLink created = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).AddLinkSuccess(parser, linkName, linkUrl);
        Assert.Equal(1, parser.OwnedLinks().Amount().Read());
        Assert.True(new CompareLinkIdentityByName(created, linkName));
        Assert.True(new CompareParserLinkUrl(created, linkUrl));
    }

    [Fact]
    private void Add_Parser_Link_Name_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        string domain = "Test";
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test parser", "Техника", domain);
        string linkName = string.Empty;
        string linkUrl = "Test Url";
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AddLinkFailure(
            parser,
            linkName,
            linkUrl
        );
    }

    [Fact]
    private void Add_Parser_Link_Url_Failure()
    {
        var parsers = _fixture.Parsers();
        string domain = "Test";
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test parser", "Техника", domain);
        string linkName = "Test Name";
        string linkUrl = string.Empty;
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AddLinkFailure(
            parser,
            linkName,
            linkUrl
        );
    }

    [Fact]
    private void Add_Parser_Link_Duplicate_Name_Failure()
    {
        var parsers = _fixture.Parsers();
        string domain = "Test";
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test parser", "Техника", domain);
        string linkName = "Test Name";
        string linkUrl = "Test Url";
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AddLinkSuccess(
            parser,
            linkName,
            linkUrl
        );
        string otherUrl = "Other Url";
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AddLinkFailure(
            parser,
            linkName,
            otherUrl
        );
    }

    [Fact]
    private void Add_Parser_Link_Duplicate_Url_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        string domain = "Test";
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test parser", "Техника", domain);
        string linkName = "Test Name";
        string linkUrl = "Test Url";
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AddLinkSuccess(
            parser,
            linkName,
            linkUrl
        );
        string otherName = "Other name";
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AddLinkFailure(
            parser,
            otherName,
            linkUrl
        );
    }

    [Fact]
    private void Add_Parser_Link_Not_Equal_Domain_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        string domain = "Test";
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test parser", "Техника", domain);
        string linkName = "Test Link";
        string linkUrl = "Other domain";
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AddLinkFailure(
            parser,
            linkName,
            linkUrl
        );
    }

    [Fact]
    private void Add_Parser_Link_To_WorkingParser_Failure()
    {
        IParsers parsers = _fixture.Parsers();
        string domain = "Test";
        IParser parser = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).CreateInitialParser("Test parser", "Техника", domain);
        string workingState = ParserState.Working();
        IParser working = new ParserTestingToolkit(
            _fixture.AccessLogger(),
            parsers
        ).UpdateParserSuccess(parser, workingState);
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AddLinkFailure(
            working,
            linkName,
            linkUrl
        );
    }
}
