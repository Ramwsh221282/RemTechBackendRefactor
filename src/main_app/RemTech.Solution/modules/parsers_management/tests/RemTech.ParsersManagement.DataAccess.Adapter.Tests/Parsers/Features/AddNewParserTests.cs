using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers.Features;

public sealed class AddNewParserTests : IClassFixture<DataAccessParsersFixture>
{
    private readonly DataAccessParsersFixture _fixture;

    public AddNewParserTests(DataAccessParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Add_New_Parser_Success_Async()
    {
        PgParsers parsers = _fixture.AccessPgParsers();
        Name parserName = new(NotEmptyString.New("Test Parser"));
        ParsingType type = ParsingType.New(NotEmptyString.New("Техника"));
        ParserServiceDomain domain = new(NotEmptyString.New("Test"));
        IParser parser = new Parser(parserName, type, domain);
        Status adding = await parsers.Add(parser);
        Assert.True(adding.IsSuccess);
    }

    [Fact]
    private async Task Query_Parser()
    {
        PgParsers parsers = _fixture.AccessPgParsers();
        Name parserName = new(NotEmptyString.New("Test Parser"));
        ParsingType type = ParsingType.New(NotEmptyString.New("Техника"));
        ParserServiceDomain domain = new(NotEmptyString.New("Test"));
        IParser parser = new Parser(parserName, type, domain);
        Status adding = await parsers.Add(parser);
        Assert.True(adding.IsSuccess);
        await parsers.Find(parser.Identification().ReadType(), parser.Domain());
    }
}
