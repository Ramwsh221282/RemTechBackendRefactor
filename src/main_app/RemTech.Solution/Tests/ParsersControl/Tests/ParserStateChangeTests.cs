using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.ParsersControl.Features;

namespace Tests.ParsersControl.Tests;

public sealed class ParserStateChangeTests(ParsersControlFixture fixture) : IClassFixture<ParsersControlFixture>
{
    private readonly ParserControlFeaturesFacade _facade = new(fixture.Services);
    private const string Type = "test-type";
    private const string Domain = "test-domain";

    [Fact]
    private async Task Enable_Parser_From_Creating_Failure()
    {
        Guid parserId = await CreatedParserId();
        Result<ParserStateChangeResponse> enabling = await _facade.EnableParser(parserId);
        Assert.True(enabling.IsSuccess);
    }

    [Fact]
    private async Task Enable_Already_Enabled_Parser_Failure()
    {
        Guid parserId = await CreatedParserId();
        await _facade.EnableParser(parserId);
        Result<ParserStateChangeResponse> enabling = await _facade.EnableParser(parserId);
        Assert.True(enabling.IsFailure);
    }

    [Fact]
    private async Task Make_Enabled_Parser_Waiting_Success()
    {
        Guid parserId = await CreatedParserId();
        await _facade.EnableParser(parserId);
        Result<ParserStateChangeResponse> result = await _facade.MakeWaitingParser(parserId);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Make_Disabled_Parser_Waiting_Failure()
    {
        Guid parserId = await CreatedParserId();
        Result<ParserStateChangeResponse> result = await _facade.MakeWaitingParser(parserId);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Make_Waiting_Parser_Waiting_Failure()
    {
        Guid parserId = await CreatedParserId();
        await _facade.EnableParser(parserId);
        await _facade.MakeWaitingParser(parserId);
        Result<ParserStateChangeResponse> result = await _facade.MakeWaitingParser(parserId);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Make_Waiting_Parser_Working()
    {
        Guid parserId = await CreatedParserId();
        await _facade.EnableParser(parserId);
        await _facade.MakeWaitingParser(parserId);
        Result<ParserStateChangeResponse> result = await _facade.MakeWorkingParser(parserId);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Make_Enabled_Parser_Working_Failure()
    {
        Guid parserId = await CreatedParserId();
        await _facade.EnableParser(parserId);
        Result<ParserStateChangeResponse> result = await _facade.MakeWorkingParser(parserId);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Make_Disabled_Parser_Working_Failure()
    {
        Guid parserId = await CreatedParserId();
        Result<ParserStateChangeResponse> result = await _facade.MakeWorkingParser(parserId);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Make_Working_Parser_Working_Failure()
    {
        Guid parserId = await CreatedParserId();
        await _facade.EnableParser(parserId);
        await _facade.MakeWaitingParser(parserId);
        Result<ParserStateChangeResponse> result = await _facade.MakeWaitingParser(parserId);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Disable_Disabled_Parser_Failure()
    {
        Guid parserId = await CreatedParserId();
        Result<ParserStateChangeResponse> result = await _facade.DisableParser(parserId);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Disable_Enabled_Parser_Success()
    {
        Guid parserId = await CreatedParserId();
        await _facade.EnableParser(parserId);
        Result<ParserStateChangeResponse> result = await _facade.DisableParser(parserId);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Disable_Waiting_Parser_Success()
    {
        Guid parserId = await CreatedParserId();
        await _facade.EnableParser(parserId);
        await _facade.MakeWaitingParser(parserId);
        Result<ParserStateChangeResponse> result = await _facade.DisableParser(parserId);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Disable_Working_Parser_Success()
    {
        Guid parserId = await CreatedParserId();
        await _facade.EnableParser(parserId);
        await _facade.MakeWaitingParser(parserId);
        await _facade.MakeWorkingParser(parserId);
        Result<ParserStateChangeResponse> result = await _facade.DisableParser(parserId);
        Assert.True(result.IsSuccess);
    }
    
    private async Task<Guid> CreatedParserId()
    {
        Result<AddParserResponse> parser = await _facade.AddParser(Domain, Type);
        Assert.True(parser.IsSuccess);
        Guid parserId = parser.Value.Id;
        return parserId;
    }
}