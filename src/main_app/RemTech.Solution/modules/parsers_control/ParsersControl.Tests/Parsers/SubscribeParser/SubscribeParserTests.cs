using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Tests.Parsers.SubscribeParser;

public sealed class SubscribeParserTests(
    IntegrationalTestsFixture fixture)
    : IClassFixture<IntegrationalTestsFixture>
{
    private IServiceProvider Services { get; } = fixture.Services;

    [Fact]
    private async Task Subscribe_Parser_Using_ExternalService()
    {
        string domain = "Some Domain";
        string type = "Some Type";
        Guid id = Guid.NewGuid();
        await Services.InvokeSubscriptionFromExternalService(domain, type, id);
        await Task.Delay(TimeSpan.FromSeconds(10));
        Result<SubscribedParser> parser = await Services.GetParser(id);
        Assert.True(parser.IsSuccess);
    }
    
    [Fact]
    private async Task Subscribe_Parser_Ensure_Subscribed()
    {
        string domain = "Some Domain";
        string type = "Some Type";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
        Assert.True(result.IsSuccess);
        bool saved = await Services.EnsureSaved(id);
        Assert.True(saved);
    }
    
    [Fact]
    private async Task Subscribe_Parser_Duplication_By_Domain_And_Type()
    {
        string domain = "Some Domain";
        string type = "Some Type";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
        Assert.True(result.IsSuccess);
        Result<SubscribedParser> result2 = await Services.InvokeSubscription(domain, type, id);
        Assert.False(result2.IsSuccess);
    }
    
    [Fact]
    private async Task Susbscribe_Parser_Invalid_Domain()
    {
        string domain = "";
        string type = "Some Type";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    private async Task Subscribe_Parser_Invalid_Type()
    {
        string domain = "Some Domain";
        string type = "";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    private async Task Subscribe_Parser_Invalid_Id()
    {
        string domain = "Some Domain";
        string type = "Some Type";
        Guid id = Guid.Empty;
        Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
        Assert.False(result.IsSuccess);
    }
}