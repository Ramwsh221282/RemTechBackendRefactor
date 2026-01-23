using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Tests.Parsers.SetParserLinkWorkTime;

public sealed class SetParserLinkWorkTimeTests(IntegrationalTestsFixture fixture)
    : IClassFixture<IntegrationalTestsFixture>
{
    private IServiceProvider Services { get; } = fixture.Services;

    [Fact]
    private async Task Set_Link_Work_Time_Success()
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

        const long workTimeSeconds = 3600;
        Result<SubscribedParserLink> setWorkTimeResult = await Services.SetLinkWorkTime(
            parserId,
            linkId,
            workTimeSeconds
        );

        Assert.True(setWorkTimeResult.IsSuccess);
        Assert.Equal(workTimeSeconds, setWorkTimeResult.Value.Statistics.WorkTime.TotalElapsedSeconds);

        Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
        Assert.True(parserResult.IsSuccess);
        Result<SubscribedParserLink> link = parserResult.Value.FindLink(linkId);
        Assert.True(link.IsSuccess);
        Assert.Equal(workTimeSeconds, link.Value.Statistics.WorkTime.TotalElapsedSeconds);
    }

    [Fact]
    private async Task Set_Link_Work_Time_Multiple_Times_Success()
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

        Result<SubscribedParserLink> firstSet = await Services.SetLinkWorkTime(parserId, linkId, 1800);
        Assert.True(firstSet.IsSuccess);
        Assert.Equal(1800, firstSet.Value.Statistics.WorkTime.TotalElapsedSeconds);

        Result<SubscribedParserLink> secondSet = await Services.SetLinkWorkTime(parserId, linkId, 2700);
        Assert.True(secondSet.IsSuccess);
        Assert.Equal(2700, secondSet.Value.Statistics.WorkTime.TotalElapsedSeconds);

        Result<SubscribedParserLink> thirdSet = await Services.SetLinkWorkTime(parserId, linkId, 900); // 15 minutes
        Assert.True(thirdSet.IsSuccess);
        Assert.Equal(900, thirdSet.Value.Statistics.WorkTime.TotalElapsedSeconds);

        Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
        Assert.True(parserResult.IsSuccess);
        Result<SubscribedParserLink> link = parserResult.Value.FindLink(linkId);
        Assert.True(link.IsSuccess);
        Assert.Equal(900, link.Value.Statistics.WorkTime.TotalElapsedSeconds);
    }

    [Fact]
    private async Task Set_Link_Work_Time_When_Parser_Not_Working_Failure()
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

        Result<SubscribedParserLink> setWorkTimeResult = await Services.SetLinkWorkTime(parserId, linkId, 3600);
        Assert.True(setWorkTimeResult.IsFailure);
    }

    [Fact]
    private async Task Set_Link_Work_Time_With_Invalid_Link_Id_Failure()
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
        Result<SubscribedParserLink> setWorkTimeResult = await Services.SetLinkWorkTime(parserId, invalidLinkId, 3600);

        Assert.True(setWorkTimeResult.IsFailure);
    }

    [Fact]
    private async Task Set_Link_Work_Time_With_Invalid_Parser_Id_Failure()
    {
        Guid invalidParserId = Guid.NewGuid();
        Guid linkId = Guid.NewGuid();

        Result<SubscribedParserLink> setWorkTimeResult = await Services.SetLinkWorkTime(invalidParserId, linkId, 3600);

        Assert.True(setWorkTimeResult.IsFailure);
    }

    [Fact]
    private async Task Set_Link_Work_Time_After_Parser_Stopped_Failure()
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

        Result<SubscribedParserLink> firstSet = await Services.SetLinkWorkTime(parserId, linkId, 1800);
        Assert.True(firstSet.IsSuccess);

        Result<SubscribedParser> stopResult = await Services.DisableParser(parserId);
        Assert.True(stopResult.IsSuccess);

        Result<SubscribedParserLink> setWorkTimeResult = await Services.SetLinkWorkTime(parserId, linkId, 3600);
        Assert.True(setWorkTimeResult.IsFailure);
    }

    [Fact]
    private async Task Set_Link_Work_Time_With_Zero_Seconds_Success()
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

        Result<SubscribedParserLink> setWorkTimeResult = await Services.SetLinkWorkTime(parserId, linkId, 0);
        Assert.True(setWorkTimeResult.IsSuccess);
        Assert.Equal(0, setWorkTimeResult.Value.Statistics.WorkTime.TotalElapsedSeconds);
    }

    [Fact]
    private async Task Set_Link_Work_Time_With_Large_Value_Success()
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

        const long largeWorkTime = 86400;
        Result<SubscribedParserLink> setWorkTimeResult = await Services.SetLinkWorkTime(
            parserId,
            linkId,
            largeWorkTime
        );

        Assert.True(setWorkTimeResult.IsSuccess);
        Assert.Equal(largeWorkTime, setWorkTimeResult.Value.Statistics.WorkTime.TotalElapsedSeconds);

        Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
        Assert.True(parserResult.IsSuccess);
        Result<SubscribedParserLink> link = parserResult.Value.FindLink(linkId);
        Assert.True(link.IsSuccess);
        Assert.Equal(largeWorkTime, link.Value.Statistics.WorkTime.TotalElapsedSeconds);
    }
}
