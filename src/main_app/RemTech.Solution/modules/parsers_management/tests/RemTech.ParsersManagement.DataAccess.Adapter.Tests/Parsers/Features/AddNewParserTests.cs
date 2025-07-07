using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers.Features;

public sealed class AddNewParserTests : IClassFixture<DataAccessParsersFixture>
{
    private readonly DataAccessParsersFixture _fixture;

    public AddNewParserTests(DataAccessParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private async Task Add_New_Parser_Async_Success_Async_Ensure_Created_By_Name()
    {
        ParserTestingToolkit toolkit = new(_fixture.Logger(), _fixture.ParsersSource());
        string name = "Test Parser";
        string type = "Техника";
        string domain = "Test";
        await toolkit.AsyncAddNewParserSuccess(name, type, domain);
        await using ParsersSource parsers = _fixture.ParsersSource();
        Status<IParser> created = await parsers.Find(new Name(NotEmptyString.New(name)));
        Assert.True(created.IsSuccess);
        Assert.Equal(name, created.Value.Identification().ReadName().NameString().StringValue());
        Assert.Equal(type, created.Value.Identification().ReadType().Read().StringValue());
        Assert.Equal(domain, created.Value.Domain().Read().NameString().StringValue());
    }

    [Fact]
    private async Task Add_New_Parser_Async_Success_Ensure_Created_By_Id()
    {
        ParserTestingToolkit toolkit = new(_fixture.Logger(), _fixture.ParsersSource());
        string name = "Test Parser";
        string type = "Техника";
        string domain = "Test";
        IParser parser = await toolkit.AsyncAddNewParserSuccess(name, type, domain);
        await using ParsersSource parsers = _fixture.ParsersSource();
        Status<IParser> created = await parsers.Find(parser.Identification().ReadId());
        Assert.True(created.IsSuccess);
        Assert.Equal(name, created.Value.Identification().ReadName().NameString().StringValue());
        Assert.Equal(type, created.Value.Identification().ReadType().Read().StringValue());
        Assert.Equal(domain, created.Value.Domain().Read().NameString().StringValue());
    }

    [Fact]
    private async Task Add_New_Parser_Async_Name_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture.Logger(), _fixture.ParsersSource());
        string name = string.Empty;
        string type = "Техника";
        string domain = "Test";
        await toolkit.AsyncAddNewParserFailure(name, type, domain);
    }

    [Fact]
    private async Task Add_New_Parser_Async_Type_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture.Logger(), _fixture.ParsersSource());
        string name = "Test parser";
        string type = "Random text";
        string domain = "Test";
        await toolkit.AsyncAddNewParserFailure(name, type, domain);
    }

    [Fact]
    private async Task Add_New_Parser_Async_Domain_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture.Logger(), _fixture.ParsersSource());
        string name = "Test parser";
        string type = "Техника";
        string domain = string.Empty;
        await toolkit.AsyncAddNewParserFailure(name, type, domain);
    }

    [Fact]
    private async Task Task_Add_New_Parser_Duplicate_Domain_Type_Failure()
    {
        ParserTestingToolkit toolkit1 = new(_fixture.Logger(), _fixture.ParsersSource());
        string name1 = "Test Parser";
        string type1 = "Техника";
        string domain1 = "Test";
        await toolkit1.AsyncAddNewParserSuccess(name1, type1, domain1);
        ParserTestingToolkit toolkit2 = new(_fixture.Logger(), _fixture.ParsersSource());
        string name2 = "Other name";
        string type2 = "Техника";
        string domain2 = "Test";
        await toolkit2.AsyncAddNewParserFailure(name2, type2, domain2);
    }
}
