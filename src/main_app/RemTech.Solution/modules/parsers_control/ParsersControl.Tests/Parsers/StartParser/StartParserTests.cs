using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Tests.Parsers.StartParser;

public sealed class StartParserTests(IntegrationalTestsFixture fixture) : IClassFixture<IntegrationalTestsFixture>
{
    private IServiceProvider Services { get; } = fixture.Services;
    
    [Fact]
    public async Task Start_After_Disabled_Success()
    {
        string type = "Some Type";
        string domain = "Some Domain";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
        Assert.True(result.IsSuccess);
        Result<SubscribedParser> enabled = await Services.EnableParser(id);
        Assert.True(enabled.IsSuccess);
        Result<SubscribedParser> started = await Services.StartParser(id);
        Assert.True(started.IsSuccess);
    }

    [Fact]
    public async Task Start_Parser_Disabled_Failure()
    {
        string type = "Some Type";
        string domain = "Some Domain";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
        Assert.True(result.IsSuccess);
        Result<SubscribedParser> started = await Services.StartParser(id);
        Assert.True(started.IsFailure);
    }
}