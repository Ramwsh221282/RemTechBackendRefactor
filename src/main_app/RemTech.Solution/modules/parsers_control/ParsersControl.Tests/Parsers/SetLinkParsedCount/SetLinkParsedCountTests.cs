using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Tests.Parsers.SetLinkParsedCount;

public sealed class SetLinkParsedCountTests(IntegrationalTestsFixture fixture)
    : IClassFixture<IntegrationalTestsFixture>
{
    private IServiceProvider Services { get; } = fixture.Services;

    [Fact]
    private async Task Set_Link_Parsed_Count_Success()
    {
        const string domain = "Some domain";
        const string type = "Some type";
        Guid parserId = Guid.NewGuid();

        Result<SubscribedParser> subscribeResult = await Services.InvokeSubscription(domain, type, parserId);
        Assert.True(subscribeResult.IsSuccess);

        Result<IEnumerable<SubscribedParserLink>> linkResult = await Services.AddLink(
            parserId,
            "https://example.com",
            "Test Link"
        );
        Assert.True(linkResult.IsSuccess);
        Guid linkId = linkResult.Value.First().Id.Value;

        Result<SubscribedParser> enableResult = await Services.EnableParser(parserId);
        Assert.True(enableResult.IsSuccess);

        Result<SubscribedParser> startResult = await Services.StartParser(parserId);
        Assert.True(startResult.IsSuccess);

        const int parsedAmount = 100;
        Result<SubscribedParserLink> setParsedCountResult = await Services.SetLinkParsedAmount(
            parserId,
            linkId,
            parsedAmount
        );

        Assert.True(setParsedCountResult.IsSuccess);
        Assert.Equal(parsedAmount, setParsedCountResult.Value.Statistics.ParsedCount.Value);

        Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
        Assert.True(parserResult.IsSuccess);
        Result<SubscribedParserLink> link = parserResult.Value.FindLink(linkId);
        Assert.True(link.IsSuccess);
        Assert.Equal(parsedAmount, link.Value.Statistics.ParsedCount.Value);
    }

    [Fact]
    private async Task Set_Link_Parsed_Count_Multiple_Times_Success()
    {
        const string domain = "Some domain";
        const string type = "Some type";
        Guid parserId = Guid.NewGuid();

        Result<SubscribedParser> subscribeResult = await Services.InvokeSubscription(domain, type, parserId);
        Assert.True(subscribeResult.IsSuccess);

        Result<IEnumerable<SubscribedParserLink>> linkResult = await Services.AddLink(
            parserId,
            "https://example.com",
            "Test Link"
        );
        Assert.True(linkResult.IsSuccess);
        Guid linkId = linkResult.Value.First().Id.Value;

        Result<SubscribedParser> enableResult = await Services.EnableParser(parserId);
        Assert.True(enableResult.IsSuccess);

        Result<SubscribedParser> startResult = await Services.StartParser(parserId);
        Assert.True(startResult.IsSuccess);

        Result<SubscribedParserLink> firstSet = await Services.SetLinkParsedAmount(parserId, linkId, 50);
        Assert.True(firstSet.IsSuccess);
        Assert.Equal(50, firstSet.Value.Statistics.ParsedCount.Value);

        Result<SubscribedParserLink> secondSet = await Services.SetLinkParsedAmount(parserId, linkId, 75);
        Assert.True(secondSet.IsSuccess);
        Assert.Equal(125, secondSet.Value.Statistics.ParsedCount.Value);

        Result<SubscribedParserLink> thirdSet = await Services.SetLinkParsedAmount(parserId, linkId, 25);
        Assert.True(thirdSet.IsSuccess);
        Assert.Equal(150, thirdSet.Value.Statistics.ParsedCount.Value);

        Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
        Assert.True(parserResult.IsSuccess);
        Result<SubscribedParserLink> link = parserResult.Value.FindLink(linkId);
        Assert.True(link.IsSuccess);
        Assert.Equal(150, link.Value.Statistics.ParsedCount.Value);
    }

    [Fact]
    private async Task Set_Link_Parsed_Count_When_Parser_Not_Working_Failure()
    {
        const string domain = "Some domain";
        const string type = "Some type";
        Guid parserId = Guid.NewGuid();

        Result<SubscribedParser> subscribeResult = await Services.InvokeSubscription(domain, type, parserId);
        Assert.True(subscribeResult.IsSuccess);

        Result<IEnumerable<SubscribedParserLink>> linkResult = await Services.AddLink(
            parserId,
            "https://example.com",
            "Test Link"
        );
        Assert.True(linkResult.IsSuccess);
        Guid linkId = linkResult.Value.First().Id.Value;

        Result<SubscribedParserLink> setParsedCountResult = await Services.SetLinkParsedAmount(parserId, linkId, 100);

        Assert.True(setParsedCountResult.IsFailure);
    }

    [Fact]
    private async Task Set_Link_Parsed_Count_With_Invalid_Link_Id_Failure()
    {
        const string domain = "Some domain";
        const string type = "Some type";
        Guid parserId = Guid.NewGuid();

        Result<SubscribedParser> subscribeResult = await Services.InvokeSubscription(domain, type, parserId);
        Assert.True(subscribeResult.IsSuccess);

        Result<SubscribedParser> enableResult = await Services.EnableParser(parserId);
        Assert.True(enableResult.IsSuccess);

        Result<SubscribedParser> startResult = await Services.StartParser(parserId);
        Assert.True(startResult.IsSuccess);

        Guid invalidLinkId = Guid.NewGuid();
        Result<SubscribedParserLink> setParsedCountResult = await Services.SetLinkParsedAmount(
            parserId,
            invalidLinkId,
            100
        );

        Assert.True(setParsedCountResult.IsFailure);
    }

    [Fact]
    private async Task Set_Link_Parsed_Count_With_Invalid_Parser_Id_Failure()
    {
        Guid invalidParserId = Guid.NewGuid();
        Guid linkId = Guid.NewGuid();

        Result<SubscribedParserLink> setParsedCountResult = await Services.SetLinkParsedAmount(
            invalidParserId,
            linkId,
            100
        );

        Assert.True(setParsedCountResult.IsFailure);
    }

    [Fact]
    private async Task Set_Link_Parsed_Count_After_Parser_Stopped_Failure()
    {
        const string domain = "Some domain";
        const string type = "Some type";
        Guid parserId = Guid.NewGuid();

        Result<SubscribedParser> subscribeResult = await Services.InvokeSubscription(domain, type, parserId);
        Assert.True(subscribeResult.IsSuccess);

        Result<IEnumerable<SubscribedParserLink>> linkResult = await Services.AddLink(
            parserId,
            "https://example.com",
            "Test Link"
        );
        Assert.True(linkResult.IsSuccess);
        Guid linkId = linkResult.Value.First().Id.Value;

        Result<SubscribedParser> enableResult = await Services.EnableParser(parserId);
        Assert.True(enableResult.IsSuccess);

        Result<SubscribedParser> startResult = await Services.StartParser(parserId);
        Assert.True(startResult.IsSuccess);

        Result<SubscribedParserLink> firstSet = await Services.SetLinkParsedAmount(parserId, linkId, 50);
        Assert.True(firstSet.IsSuccess);

        Result<SubscribedParser> stopResult = await Services.DisableParser(parserId);
        Assert.True(stopResult.IsSuccess);

        Result<SubscribedParserLink> setParsedCountResult = await Services.SetLinkParsedAmount(parserId, linkId, 100);

        Assert.True(setParsedCountResult.IsFailure);
    }
}
