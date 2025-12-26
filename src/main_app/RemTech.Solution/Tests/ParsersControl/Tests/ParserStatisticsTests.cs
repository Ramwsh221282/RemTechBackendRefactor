using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.ParsersControl.Features;

namespace Tests.ParsersControl.Tests;

public sealed class ParserStatisticsTests(ParsersControlFixture fixture) : IClassFixture<ParsersControlFixture>
{
    private readonly ParserControlFeaturesFacade _facade = new(fixture.Services);
    
    [Fact]
    private async Task Update_Processed_Success()
    {
        const string domain = "test-domain";
        const string type = "test-type";
        const int nextProcessed = 50;
        Result<AddParserResponse> parser = await _facade.AddParser(domain, type);
        Guid parserId = parser.Value.Id;
        Result<ParserStatisticsUpdateResponse> updating = await _facade.UpdateProcessed(parserId, nextProcessed);
        Assert.True(updating.IsSuccess);
        bool equals = await _facade.ProcessedEqualsTo(parserId, nextProcessed);
        Assert.True(equals);
    }

    [Fact]
    private async Task Update_Elapsed_Success()
    {
        const string domain = "test-domain";
        const string type = "test-type";
        const long nextElapsed = 12312312;
        Result<AddParserResponse> parser = await _facade.AddParser(domain, type);
        Guid parserId = parser.Value.Id;
        Result<ParserStatisticsUpdateResponse> updating = await _facade.UpdateElapsedSeconds(parserId, nextElapsed);
        Assert.True(updating.IsSuccess);
        bool equals = await _facade.ElapsedEqualsTo(parserId, nextElapsed);
        Assert.True(equals);
    }

    [Fact]
    private async Task Update_Elapsed_Invalid_Elapsed()
    {
        const string domain = "test-domain";
        const string type = "test-type";
        const long nextElapsed = -12312312;
        Result<AddParserResponse> parser = await _facade.AddParser(domain, type);
        Guid parserId = parser.Value.Id;
        Result<ParserStatisticsUpdateResponse> updating = await _facade.UpdateElapsedSeconds(parserId, nextElapsed);
        Assert.True(updating.IsFailure);
    }

    [Fact]
    private async Task Update_Elapsed_Not_Found()
    {
        const long nextElapsed = -12312312;
        Guid parserId = Guid.NewGuid();
        Result<ParserStatisticsUpdateResponse> updating = await _facade.UpdateElapsedSeconds(parserId, nextElapsed);
        Assert.True(updating.IsFailure);
    }
    
    [Fact]
    private async Task Update_Processed_Parser_Not_Found()
    {
        const int nextProcessed = 50;
        Guid parserId = Guid.NewGuid();
        Result<ParserStatisticsUpdateResponse> updating = await _facade.UpdateProcessed(parserId, nextProcessed);
        Assert.True(updating.IsFailure);
    }
    
    [Fact]
    private async Task Update_Processed_Invalid_Processed()
    {
        const string domain = "test-domain";
        const string type = "test-type";
        const int nextProcessed = -50;
        Result<AddParserResponse> parser = await _facade.AddParser(domain, type);
        Guid parserId = parser.Value.Id;
        Result<ParserStatisticsUpdateResponse> updating = await _facade.UpdateProcessed(parserId, nextProcessed);
        Assert.True(updating.IsFailure);
    }
}