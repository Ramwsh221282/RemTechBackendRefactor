using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers.Features;

public sealed class StartParserTests : IClassFixture<DataAccessParsersFixture>
{
    private readonly DataAccessParsersFixture _fixture;

    public StartParserTests(DataAccessParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private async Task Start_Parser_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        string expectedState = ParserState.Working();
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
        await using IParsers source = _fixture.Parsers();
        Status<IParser> fromDb = await source.Find(parser.Identification().ReadId());
        Assert.True(new CompareParserState(fromDb.Value, expectedState));
    }

    [Fact]
    private async Task Start_Working_Parser_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        string workingState = ParserState.Working();
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
        ).UpdateParserAsyncSuccess(parser, workingState);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).StartParserFailureAsync(parser.Identification().ReadId());
    }

    [Fact]
    private async Task Start_Parser_With_All_Inactive_Links_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test";
        string linkName = "Test Link";
        string linkUrl = "Test Url";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(_fixture.Logger(), _fixture.Parsers()).AddLinkSuccessAsync(
            parser.Identification().ReadId(),
            linkName,
            linkUrl
        );
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).EnableParserSuccessAsync(parser);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).StartParserFailureAsync(parser.Identification().ReadId());
    }

    [Fact]
    private async Task Start_Parser_Without_Links_Failure()
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
        ).EnableParserSuccessAsync(parser);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).StartParserFailureAsync(parser.Identification().ReadId());
    }
}
