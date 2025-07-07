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
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).ChangeLinkActivitySuccessAsync(
            parser.Identification().ReadId(),
            link.Identification().ReadId(),
            true
        );
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).EnableParserSuccessAsync(parser);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).StartParserSuccessAsync(parser.Identification().ReadId());
        await using ParsersSource source = _fixture.ParsersSource();
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
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParserLink link = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).ChangeLinkActivitySuccessAsync(
            parser.Identification().ReadId(),
            link.Identification().ReadId(),
            true
        );
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).EnableParserSuccessAsync(parser);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).UpdateParserAsyncSuccess(parser, workingState);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
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
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).AddLinkSuccessAsync(parser.Identification().ReadId(), linkName, linkUrl);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).EnableParserSuccessAsync(parser);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
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
            _fixture.ParsersSource()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).EnableParserSuccessAsync(parser);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.ParsersSource()
        ).StartParserFailureAsync(parser.Identification().ReadId());
    }
}
