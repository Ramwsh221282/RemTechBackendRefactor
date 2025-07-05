using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkUrls.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public sealed class AddParserLinkTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public AddParserLinkTests(ParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private void Add_Parser_Link_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string domain = "Test";
        IParser parser = toolkit.CreateInitialParser("Test parser", "Техника", domain);
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        IParserLink created = toolkit.AddLinkSuccess(parser, linkName, linkUrl);
        Assert.Equal(1, parser.OwnedLinks().Amount().Read());
        Assert.True(new CompareLinkIdentityByName(created, linkName));
        Assert.True(new CompareParserLinkUrl(created, linkUrl));
    }

    [Fact]
    private void Add_Parser_Link_Name_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string domain = "Test";
        IParser parser = toolkit.CreateInitialParser("Test parser", "Техника", domain);
        string linkName = string.Empty;
        string linkUrl = "Test Url";
        toolkit.AddLinkFailure(parser, linkName, linkUrl);
    }

    [Fact]
    private void Add_Parser_Link_Url_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string domain = "Test";
        IParser parser = toolkit.CreateInitialParser("Test parser", "Техника", domain);
        string linkName = "Test Name";
        string linkUrl = string.Empty;
        toolkit.AddLinkFailure(parser, linkName, linkUrl);
    }

    [Fact]
    private void Add_Parser_Link_Duplicate_Name_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string domain = "Test";
        IParser parser = toolkit.CreateInitialParser("Test parser", "Техника", domain);
        string linkName = "Test Name";
        string linkUrl = "Test Url";
        toolkit.AddLinkSuccess(parser, linkName, linkUrl);
        string otherUrl = "Other Url";
        toolkit.AddLinkFailure(parser, linkName, otherUrl);
    }

    [Fact]
    private void Add_Parser_Link_Duplicate_Url_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string domain = "Test";
        IParser parser = toolkit.CreateInitialParser("Test parser", "Техника", domain);
        string linkName = "Test Name";
        string linkUrl = "Test Url";
        toolkit.AddLinkSuccess(parser, linkName, linkUrl);
        string otherName = "Other name";
        toolkit.AddLinkFailure(parser, otherName, linkUrl);
    }

    [Fact]
    private void Add_Parser_Link_Not_Equal_Domain_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string domain = "Test";
        IParser parser = toolkit.CreateInitialParser("Test parser", "Техника", domain);
        string linkName = "Test Link";
        string linkUrl = "Other domain";
        toolkit.AddLinkFailure(parser, linkName, linkUrl);
    }

    [Fact]
    private void Add_Parser_Link_To_WorkingParser_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        string domain = "Test";
        IParser parser = toolkit.CreateInitialParser("Test parser", "Техника", domain);
        string workingState = ParserState.Working();
        IParser working = toolkit.UpdateParserSuccess(parser, workingState);
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        toolkit.AddLinkFailure(working, linkName, linkUrl);
    }
}
