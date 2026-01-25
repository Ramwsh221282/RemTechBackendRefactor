using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Tests.Parsers.AddParserLink;

public sealed class AddParserLinkTests(IntegrationalTestsFixture fixture) : IClassFixture<IntegrationalTestsFixture>
{
    private IServiceProvider Services { get; } = fixture.Services;

    [Fact]
    private async Task Add_New_Parser_Link_When_Parser_Working_Failure()
    {
        const string domain = "Some domain";
        const string type = "Some type";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
        Assert.True(result.IsSuccess);
        Result<SubscribedParser> enableResult = await Services.EnableParser(id);
        Assert.True(enableResult.IsSuccess);
        Result<IEnumerable<SubscribedParserLink>> linkResultBeforeStartWork = await Services.AddLink(
            id,
            "Test url",
            "Test name"
        );
        Assert.True(linkResultBeforeStartWork.IsSuccess);
        Result activatingLink = await Services.MakeLinkActive(id, linkResultBeforeStartWork.Value.First().Id.Value);
        Assert.True(activatingLink.IsSuccess);
        Result<SubscribedParser> startResult = await Services.StartParser(id);
        Assert.True(startResult.IsSuccess);
        Result<IEnumerable<SubscribedParserLink>> linkResult = await Services.AddLink(id, "Some url", "Some name");
        Assert.True(linkResult.IsFailure);
    }

    [Fact]
    private async Task Add_New_Parser_Link_Duplicate_Link_Name_Failure()
    {
        const string domain = "Some domain";
        const string type = "Some type";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
        Assert.True(result.IsSuccess);
        Result<IEnumerable<SubscribedParserLink>> linkResult = await Services.AddLink(id, "Some url", "Some name");
        Assert.True(linkResult.IsSuccess);
        Result<IEnumerable<SubscribedParserLink>> duplicateLinkResult = await Services.AddLink(
            id,
            "Some other url",
            "Some name"
        );
        Assert.True(duplicateLinkResult.IsFailure);
    }

    [Fact]
    private async Task Add_New_Parser_Link_Duplicate_Link_Url_Failure()
    {
        const string domain = "Some domain";
        const string type = "Some type";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
        Assert.True(result.IsSuccess);
        Result<IEnumerable<SubscribedParserLink>> linkResult = await Services.AddLink(id, "Some url", "Some name");
        Assert.True(linkResult.IsSuccess);
        Result<IEnumerable<SubscribedParserLink>> duplicateLinkResult = await Services.AddLink(
            id,
            "Some url",
            "Some other name"
        );
        Assert.True(duplicateLinkResult.IsFailure);
    }

    [Fact]
    private async Task Add_New_Parser_Link_Success()
    {
        const string domain = "Some domain";
        const string type = "Some type";
        Guid id = Guid.NewGuid();
        Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
        Assert.True(result.IsSuccess);
        Result<IEnumerable<SubscribedParserLink>> linkResult = await Services.AddLink(id, "Some url", "Some name");
        Assert.True(linkResult.IsSuccess);
        Result<SubscribedParser> parserResult = await Services.GetParser(id);
        Assert.True(parserResult.IsSuccess);
        Result<SubscribedParserLink> link = parserResult.Value.FindLink(linkResult.Value.First().Id);
        Assert.True(link.IsSuccess);
    }
}
