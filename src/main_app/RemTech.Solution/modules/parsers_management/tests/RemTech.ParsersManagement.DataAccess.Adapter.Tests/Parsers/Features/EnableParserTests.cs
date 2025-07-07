using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers.Features;

public sealed class EnableParserTests : IClassFixture<DataAccessParsersFixture>
{
    private readonly DataAccessParsersFixture _fixture;

    public EnableParserTests(DataAccessParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Enable_Parser_Async_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string expectedState = ParserState.Waiting();
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).EnableParserSuccessAsync(parser);
        await using ParsersSource source = _fixture.ParsersSource();
        Status<IParser> fromDb = await source.Find(parser.Identification().ReadId());
        Assert.True(new CompareParserState(fromDb.Value, expectedState));
    }

    [Fact]
    private async Task Enable_Enabled_Parser_Async_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).EnableParserSuccessAsync(parser);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).EnableParserFailureAsync(parser);
    }

    [Fact]
    private async Task Enable_Working_Parser_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).UpdateParserAsyncSuccess(parser, state: ParserState.Working());
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).EnableParserFailureAsync(parser);
    }
}
