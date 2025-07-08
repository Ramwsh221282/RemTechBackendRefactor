using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests.Features;

public sealed class AddNewParserTests : IClassFixture<CacheAdapterParsersFixture>
{
    private readonly CacheAdapterParsersFixture _fixture;

    public AddNewParserTests(CacheAdapterParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Add_New_Parser_Success_Ensure_Same_As_From_Cache()
    {
        string name1 = "Test Parser";
        string type1 = "Техника";
        string domain1 = "Test";
        string name2 = "Other Parser";
        string type2 = "Запчасти";
        string domain2 = "Other";
        await _fixture.Toolkit().AsyncAddNewParserSuccess(name1, type1, domain1);
        await _fixture.Toolkit().AsyncAddNewParserSuccess(name2, type2, domain2);
        await using ParsersSource parsers = _fixture.Parsers();
        Status<IParser> created1 = await parsers.Find(new Name(NotEmptyString.New(name1)));
        Status<IParser> created2 = await parsers.Find(new Name(NotEmptyString.New(name2)));
        Assert.True(created1.IsSuccess);
        Assert.True(created2.IsSuccess);
        string createdName1 = created1.Value.Identification().ReadName();
        string createdType1 = created1.Value.Identification().ReadType().Read();
        string createdDomain1 = created1.Value.Identification().Domain();
        string createdName2 = created2.Value.Identification().ReadName();
        string createdType2 = created2.Value.Identification().ReadType().Read();
        string createdDomain2 = created2.Value.Identification().Domain();
        Assert.Equal(name1, createdName1);
        Assert.Equal(type1, createdType1);
        Assert.Equal(domain1, createdDomain1);
        Assert.Equal(name2, createdName2);
        Assert.Equal(type2, createdType2);
        Assert.Equal(domain2, createdDomain2);
        IParser[] cached = await _fixture.CachedSource().Get();
        Assert.NotEmpty(cached);
        IParser? fromCache1 = cached.FirstOrDefault(c =>
            c.Identification().ReadId().Equals(created1.Value.Identification().ReadId())
        );
        Assert.NotNull(fromCache1);
        IParser? fromCache2 = cached.FirstOrDefault(c =>
            c.Identification().ReadId().Equals(created2.Value.Identification().ReadId())
        );
        Assert.NotNull(fromCache2);
        string fromCacheArrayName1 = fromCache1.Identification().ReadName();
        string fromCacheArrayType1 = fromCache1.Identification().ReadType().Read();
        string fromCacheArrayDomain1 = fromCache1.Identification().Domain();
        string fromCacheArrayName2 = fromCache2.Identification().ReadName();
        string fromCacheArrayType2 = fromCache2.Identification().ReadType().Read();
        string fromCacheArrayDomain2 = fromCache2.Identification().Domain();
        Assert.Equal(name1, fromCacheArrayName1);
        Assert.Equal(type1, fromCacheArrayType1);
        Assert.Equal(domain1, fromCacheArrayDomain1);
        Assert.Equal(name2, fromCacheArrayName2);
        Assert.Equal(type2, fromCacheArrayType2);
        Assert.Equal(domain2, fromCacheArrayDomain2);
    }

    [Fact]
    private async Task Add_New_Parser_Async_Success_Async_Ensure_Created_By_Name()
    {
        ParserTestingToolkit toolkit = new(_fixture.Logger(), _fixture.Parsers());
        string name = "Test Parser";
        string type = "Техника";
        string domain = "Test";
        await toolkit.AsyncAddNewParserSuccess(name, type, domain);
        await using ParsersSource parsers = _fixture.Parsers();
        Status<IParser> created = await parsers.Find(new Name(NotEmptyString.New(name)));
        Assert.True(created.IsSuccess);
        Assert.Equal(name, created.Value.Identification().ReadName().NameString().StringValue());
        Assert.Equal(type, created.Value.Identification().ReadType().Read().StringValue());
        Assert.Equal(domain, created.Value.Domain().Read().NameString().StringValue());
    }

    [Fact]
    private async Task Add_New_Parser_Async_Success_Ensure_Created_By_Id()
    {
        ParserTestingToolkit toolkit = new(_fixture.Logger(), _fixture.Parsers());
        string name = "Test Parser";
        string type = "Техника";
        string domain = "Test";
        IParser parser = await toolkit.AsyncAddNewParserSuccess(name, type, domain);
        await using ParsersSource parsers = _fixture.Parsers();
        Status<IParser> created = await parsers.Find(parser.Identification().ReadId());
        Assert.True(created.IsSuccess);
        Assert.Equal(name, created.Value.Identification().ReadName().NameString().StringValue());
        Assert.Equal(type, created.Value.Identification().ReadType().Read().StringValue());
        Assert.Equal(domain, created.Value.Domain().Read().NameString().StringValue());
    }

    [Fact]
    private async Task Add_New_Parser_Async_Name_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture.Logger(), _fixture.Parsers());
        string name = string.Empty;
        string type = "Техника";
        string domain = "Test";
        await toolkit.AsyncAddNewParserFailure(name, type, domain);
    }

    [Fact]
    private async Task Add_New_Parser_Async_Type_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture.Logger(), _fixture.Parsers());
        string name = "Test parser";
        string type = "Random text";
        string domain = "Test";
        await toolkit.AsyncAddNewParserFailure(name, type, domain);
    }

    [Fact]
    private async Task Add_New_Parser_Async_Domain_Failure()
    {
        ParserTestingToolkit toolkit = new(_fixture.Logger(), _fixture.Parsers());
        string name = "Test parser";
        string type = "Техника";
        string domain = string.Empty;
        await toolkit.AsyncAddNewParserFailure(name, type, domain);
    }

    [Fact]
    private async Task Task_Add_New_Parser_Duplicate_Domain_Type_Failure()
    {
        ParserTestingToolkit toolkit1 = new(_fixture.Logger(), _fixture.Parsers());
        string name1 = "Test Parser";
        string type1 = "Техника";
        string domain1 = "Test";
        await toolkit1.AsyncAddNewParserSuccess(name1, type1, domain1);
        ParserTestingToolkit toolkit2 = new(_fixture.Logger(), _fixture.Parsers());
        string name2 = "Other name";
        string type2 = "Техника";
        string domain2 = "Test";
        await toolkit2.AsyncAddNewParserFailure(name2, type2, domain2);
    }
}
