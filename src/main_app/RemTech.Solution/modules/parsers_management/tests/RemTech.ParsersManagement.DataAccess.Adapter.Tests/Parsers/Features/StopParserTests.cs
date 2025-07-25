using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers.Features;

public sealed class StopParserTests : IClassFixture<DataAccessParsersFixture>
{
    private readonly DataAccessParsersFixture _fixture;

    public StopParserTests(DataAccessParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Stop_Parser_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        string expectedState = ParserState.Waiting();
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).ChangeLinkActivitySuccessAsync(
            parser.Identification().ReadId(),
            link.Identification().ReadId(),
            true
        );
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).EnableParserSuccessAsync(parser);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).StartParserSuccessAsync(parser.Identification().ReadId());
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncStoppedParserSuccess(parser.Identification().ReadId());
        await using IParsers source = _fixture.Parsers();
        Status<IParser> fromDb = await source.Find(parser.Identification().ReadId());
        Assert.True(new CompareParserState(fromDb.Value, expectedState));
    }

    [Fact]
    private async Task Stop_Parser_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncStoppedParserFailure(parser.Identification().ReadId());
    }
}
