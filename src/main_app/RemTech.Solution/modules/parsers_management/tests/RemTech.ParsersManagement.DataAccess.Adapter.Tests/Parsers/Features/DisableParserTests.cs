﻿using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers.Features;

public sealed class DisableParserTests : IClassFixture<DataAccessParsersFixture>
{
    private readonly DataAccessParsersFixture _fixture;

    public DisableParserTests(DataAccessParsersFixture fixture) => _fixture = fixture;

    [Fact]
    private async Task Disable_Parser_Async_Success()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test Domain";
        string expectedState = ParserState.Disabled();
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        IParser enabled = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).UpdateParserAsyncSuccess(parser, state: ParserState.Waiting());
        IParser disabled = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).DisableParserSuccessAsync(enabled.Identification().ReadId());
        await using IParsers source = _fixture.Parsers();
        Status<IParser> fromDb = await source.Find(disabled.Identification().ReadId());
        Assert.True(new CompareParserState(fromDb.Value, expectedState));
    }

    [Fact]
    private async Task Disable_Disabled_Parser_Failure()
    {
        string parserName = "Test Parser";
        string parserType = "Техника";
        string parserDomain = "Test Domain";
        IParser parser = await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).AsyncAddNewParserSuccess(parserName, parserType, parserDomain);
        await new ParserTestingToolkit(
            _fixture.Logger(),
            _fixture.Parsers()
        ).DisableParserFailureAsync(parser.Identification().ReadId());
    }
}
