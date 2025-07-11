using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Features;

public class AddNewParserTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public AddNewParserTests(ParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private void Add_New_Parser_Non_Async_Success() =>
        new ParserTestingToolkit(_fixture.AccessLogger(), _fixture.Parsers()).AddNewParserSuccess(
            "Test Parser",
            "Техника",
            "Test"
        );

    [Fact]
    private async Task Add_New_Parser_Async_Success()
    {
        await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess("Test Parser", "Техника", "Test");
    }

    [Fact]
    private void Add_New_Parser_Name_Failure()
    {
        new ParserTestingToolkit(_fixture.AccessLogger(), _fixture.Parsers()).AddNewParserFailure(
            string.Empty,
            "Техника",
            "Test"
        );
    }

    [Fact]
    private void Add_New_Parser_Type_Failure()
    {
        new ParserTestingToolkit(_fixture.AccessLogger(), _fixture.Parsers()).AddNewParserFailure(
            "Test Parser",
            "Random Text",
            "Test"
        );
    }

    [Fact]
    private async Task Add_New_Parser_Async_Name_Failure()
    {
        await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            _fixture.Parsers()
        ).AsyncAddNewParserFailure(string.Empty, "Техника", "Test");
    }

    [Fact]
    private async Task Add_New_Parser_Async_Type_Failure()
    {
        await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            _fixture.Parsers()
        ).AsyncAddNewParserFailure("Test Parser", "Random Text", "Test");
    }

    [Fact]
    private async Task Add_New_Parser_Domain_Failure()
    {
        await new ParserTestingToolkit(
            _fixture.AccessLogger(),
            _fixture.Parsers()
        ).AsyncAddNewParserFailure("Test Parser", "Техника", string.Empty);
    }

    [Fact]
    private async Task Add_New_Parser_Async_Failure_Duplicate_By_Name()
    {
        string name = "Test Parser";
        string type = "Техника";
        string domain = "Test";
        IParsers parsers = _fixture.Parsers();
        await new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AsyncAddNewParserSuccess(
            name,
            type,
            domain
        );
        await new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AsyncAddNewParserFailure(
            name,
            type,
            domain
        );
    }

    [Fact]
    private async Task Add_New_Parser_Async_Failure_Duplicate_By_Domain()
    {
        string name = "Test Parser";
        string type = "Техника";
        string domain = "Test";
        IParsers parsers = _fixture.Parsers();
        await new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AsyncAddNewParserSuccess(
            name,
            type,
            domain
        );
        string otherName = "Other name";
        await new ParserTestingToolkit(_fixture.AccessLogger(), parsers).AsyncAddNewParserFailure(
            otherName,
            type,
            domain
        );
    }
}
