using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.ParsersControl.Features;

namespace Tests.ParsersControl.Tests;

public sealed class ParserScheduleTests(ParsersControlFixture fixture) : IClassFixture<ParsersControlFixture>
{
    private readonly ParserControlFeaturesFacade _facade = new(fixture.Services);

    [Fact]
    private async Task Set_Finished_Date_Success()
    {
        Guid parserId = await GetParserId();
        DateTime finishDate = DateTime.UtcNow;
        Result<ParserScheduleUpdateResponse> update = await _facade.SetFinishedAt(parserId, finishDate);
        Assert.True(update.IsSuccess);
    }

    [Fact]
    private async Task Set_Finished_Date_Parser_Not_Found_Failure()
    {
        Guid id = Guid.NewGuid();
        DateTime finishDate = DateTime.UtcNow;
        Result<ParserScheduleUpdateResponse> update = await _facade.SetFinishedAt(id, finishDate);
        Assert.True(update.IsFailure);
    }

    [Fact]
    private async Task Update_Wait_Days_Success()
    {
        Guid id = await GetParserId();
        int waitDays = 3;
        Result<ParserScheduleUpdateResponse> update = await _facade.UpdateWaitDays(id, waitDays);
        Assert.True(update.IsSuccess);
    }

    [Fact]
    private async Task Update_Wait_Days_Parser_Not_Found_Failure()
    {
        Guid id = Guid.NewGuid();
        int waitDays = 3;
        Result<ParserScheduleUpdateResponse> update = await _facade.UpdateWaitDays(id, waitDays);
        Assert.True(update.IsFailure);
    }
    
    [Fact]
    private async Task Update_Wait_Days_Invalid_Failure()
    {
        Guid id = await GetParserId();
        const int waitDays = -1;
        Result<ParserScheduleUpdateResponse> update = await _facade.UpdateWaitDays(id, waitDays);
        Assert.True(update.IsFailure);
    }
    
    [Fact]
    private async Task Update_Wait_Days_Greater_Than_Week_Failure()
    {
        Guid id = await GetParserId();
        const int waitDays = 8;
        Result<ParserScheduleUpdateResponse> update = await _facade.UpdateWaitDays(id, waitDays);
        Assert.True(update.IsFailure);
    }

    private async Task<Guid> GetParserId()
    {
        const string domain = "some-domain";
        const string type = "some-type";
        Result<AddParserResponse> parser = await _facade.AddParser(domain, type);
        return parser.Value.Id;
    }
}