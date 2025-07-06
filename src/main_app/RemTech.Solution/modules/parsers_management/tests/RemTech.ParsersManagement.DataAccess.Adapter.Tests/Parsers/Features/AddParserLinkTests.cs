using RemTech.ParsersManagement.Tests.Library;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers.Features;

public sealed class AddParserLinkTests : IClassFixture<DataAccessParsersFixture>
{
    private readonly DataAccessParsersFixture _fixture;

    public AddParserLinkTests(DataAccessParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private async Task Add_Parser_Link_Async_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture.Logger(), _fixture.ParsersSource());
    }
}
