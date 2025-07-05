using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public class AddNewParserTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public AddNewParserTests(ParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private void Add_New_Parser_Non_Async_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        toolkit.AddNewParserSuccess("Test Parser", "Техника", "Test");
    }

    [Fact]
    private async Task Add_New_Parser_Async_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        await toolkit.AsyncAddNewParserSuccess("Test Parser", "Техника", "Test");
    }

    [Fact]
    private async Task Add_New_Parse_Async_Success_Ensure_Created_By_Name()
    {
        string name = "Test Parser";
        string type = "Техника";
        string domain = "Test";
        ParserTestingToolkit toolkit = new(_fixture);
        await toolkit.AsyncAddNewParserSuccess(name, type, domain);
        await toolkit.ReadFromDataSource(name);
    }

    [Fact]
    private async Task Add_New_Parser_Success_Ensure_Created_By_Id()
    {
        string name = "Test Parser";
        string type = "Техника";
        string domain = "Test";
        ParserTestingToolkit toolkit = new(_fixture);
        IParser parser = await toolkit.AsyncAddNewParserSuccess(name, type, domain);
        Guid created = parser.Identification().ReadId();
        await toolkit.ReadFromDataSource(created);
    }

    [Fact]
    private void Add_New_Parser_Name_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        toolkit.AddNewParserFailure(string.Empty, "Техника", "Test");
    }

    [Fact]
    private void Add_New_Parser_Type_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        toolkit.AddNewParserFailure("Test Parser", "Random Text", "Test");
    }

    [Fact]
    private async Task Add_New_Parser_Async_Name_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        await toolkit.AsyncAddNewParserFailure(string.Empty, "Техника", "Test");
    }

    [Fact]
    private async Task Add_New_Parser_Async_Type_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        await toolkit.AsyncAddNewParserFailure("Test Parser", "Random Text", "Test");
    }

    [Fact]
    private async Task Add_New_Parser_Domain_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture);
        await toolkit.AsyncAddNewParserFailure("Test Parser", "Техника", string.Empty);
    }

    [Fact]
    private async Task Add_New_Parser_Async_Failure_Duplicate_By_Name()
    {
        string name = "Test Parser";
        string type = "Техника";
        string domain = "Test";
        ParserTestingToolkit toolkit = new(_fixture);
        await toolkit.AsyncAddNewParserSuccess(name, type, domain);
        await toolkit.AsyncAddNewParserFailure(name, type, domain);
    }

    [Fact]
    private async Task Add_New_Parser_Async_Failure_Duplicate_By_Domain()
    {
        string name = "Test Parser";
        string type = "Техника";
        string domain = "Test";
        ParserTestingToolkit toolkit = new(_fixture);
        await toolkit.AsyncAddNewParserSuccess(name, type, domain);
        string otherName = "Other name";
        await toolkit.AsyncAddNewParserFailure(otherName, type, domain);
    }
}
