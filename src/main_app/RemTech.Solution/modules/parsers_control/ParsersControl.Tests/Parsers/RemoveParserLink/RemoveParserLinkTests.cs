using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Tests.Parsers.RemoveParserLink;

public sealed class RemoveParserLinkTests(IntegrationalTestsFixture fixture) : IClassFixture<IntegrationalTestsFixture>
{
	private IServiceProvider Services { get; } = fixture.Services;

	[Fact]
	private async Task Remove_Link_When_Parser_Disabled_Success()
	{
		const string domain = "Some domain";
		const string type = "Some type";
		Guid parserId = Guid.NewGuid();

		Result<SubscribedParser> subscribeResult = await Services.InvokeSubscription(domain, type, parserId);
		Assert.True(subscribeResult.IsSuccess);

		Result<IEnumerable<SubscribedParserLink>> linkResultBeforeStartWork = await Services.AddLink(
			parserId,
			"Test url",
			"Test name"
		);
		Assert.True(linkResultBeforeStartWork.IsSuccess);
		Result activatingLink = await Services.MakeLinkActive(
			parserId,
			linkResultBeforeStartWork.Value.First().Id.Value
		);
		Assert.True(activatingLink.IsSuccess);
		Guid linkId = linkResultBeforeStartWork.Value.First().Id.Value;

		Result<SubscribedParserLink> removeResult = await Services.RemoveLink(parserId, linkId);

		Assert.True(removeResult.IsSuccess);
		Assert.Equal(linkId, removeResult.Value.Id.Value);

		Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
		Assert.True(parserResult.IsSuccess);
		Result<SubscribedParserLink> findLinkResult = parserResult.Value.FindLink(linkId);
		Assert.True(findLinkResult.IsFailure);
	}

	[Fact]
	private async Task Remove_Link_When_Parser_Sleeping_Success()
	{
		const string domain = "Some domain";
		const string type = "Some type";
		Guid parserId = Guid.NewGuid();

		Result<SubscribedParser> subscribeResult = await Services.InvokeSubscription(domain, type, parserId);
		Assert.True(subscribeResult.IsSuccess);

		Result<IEnumerable<SubscribedParserLink>> linkResultBeforeStartWork = await Services.AddLink(
			parserId,
			"Test url",
			"Test name"
		);
		Assert.True(linkResultBeforeStartWork.IsSuccess);
		Result activatingLink = await Services.MakeLinkActive(
			parserId,
			linkResultBeforeStartWork.Value.First().Id.Value
		);
		Assert.True(activatingLink.IsSuccess);
		Guid linkId = linkResultBeforeStartWork.Value.First().Id.Value;

		Result<SubscribedParser> enableResult = await Services.EnableParser(parserId);
		Assert.True(enableResult.IsSuccess);

		Result<SubscribedParser> startResult = await Services.StartParser(parserId);
		Assert.True(startResult.IsSuccess);

		Result<SubscribedParser> stopResult = await Services.DisableParser(parserId);
		Assert.True(stopResult.IsSuccess);

		Result<SubscribedParserLink> removeResult = await Services.RemoveLink(parserId, linkId);

		Assert.True(removeResult.IsSuccess);
		Assert.Equal(linkId, removeResult.Value.Id.Value);

		Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
		Assert.True(parserResult.IsSuccess);
		Result<SubscribedParserLink> findLinkResult = parserResult.Value.FindLink(linkId);
		Assert.True(findLinkResult.IsFailure);
	}

	[Fact]
	private async Task Remove_Link_When_Parser_Working_Failure()
	{
		const string domain = "Some domain";
		const string type = "Some type";
		Guid parserId = Guid.NewGuid();

		Result<SubscribedParser> subscribeResult = await Services.InvokeSubscription(domain, type, parserId);
		Assert.True(subscribeResult.IsSuccess);

		Result<IEnumerable<SubscribedParserLink>> linkResultBeforeStartWork = await Services.AddLink(
			parserId,
			"Test url",
			"Test name"
		);
		Assert.True(linkResultBeforeStartWork.IsSuccess);
		Result activatingLink = await Services.MakeLinkActive(
			parserId,
			linkResultBeforeStartWork.Value.First().Id.Value
		);
		Assert.True(activatingLink.IsSuccess);
		Guid linkId = linkResultBeforeStartWork.Value.First().Id.Value;

		Result<SubscribedParser> enableResult = await Services.EnableParser(parserId);
		Assert.True(enableResult.IsSuccess);

		Result<SubscribedParser> startResult = await Services.StartParser(parserId);
		Assert.True(startResult.IsSuccess);

		Result<SubscribedParserLink> removeResult = await Services.RemoveLink(parserId, linkId);

		Assert.True(removeResult.IsFailure);

		Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
		Assert.True(parserResult.IsSuccess);
		Result<SubscribedParserLink> findLinkResult = parserResult.Value.FindLink(linkId);
		Assert.True(findLinkResult.IsSuccess);
	}

	[Fact]
	private async Task Remove_Link_With_Invalid_Link_Id_Failure()
	{
		const string domain = "Some domain";
		const string type = "Some type";
		Guid parserId = Guid.NewGuid();

		Result<SubscribedParser> subscribeResult = await Services.InvokeSubscription(domain, type, parserId);
		Assert.True(subscribeResult.IsSuccess);

		Result<IEnumerable<SubscribedParserLink>> linkResultBeforeStartWork = await Services.AddLink(
			parserId,
			"Test url",
			"Test name"
		);
		Assert.True(linkResultBeforeStartWork.IsSuccess);
		Result activatingLink = await Services.MakeLinkActive(
			parserId,
			linkResultBeforeStartWork.Value.First().Id.Value
		);
		Assert.True(activatingLink.IsSuccess);

		Guid invalidLinkId = Guid.NewGuid();
		Result<SubscribedParserLink> removeResult = await Services.RemoveLink(parserId, invalidLinkId);

		Assert.True(removeResult.IsFailure);

		Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
		Assert.True(parserResult.IsSuccess);
		Result<SubscribedParserLink> findLinkResult = parserResult.Value.FindLink(
			linkResultBeforeStartWork.Value.First().Id
		);
		Assert.True(findLinkResult.IsSuccess);
	}

	[Fact]
	private async Task Remove_Link_With_Invalid_Parser_Id_Failure()
	{
		Guid invalidParserId = Guid.NewGuid();
		Guid linkId = Guid.NewGuid();

		Result<SubscribedParserLink> removeResult = await Services.RemoveLink(invalidParserId, linkId);

		Assert.True(removeResult.IsFailure);
	}

	[Fact]
	private async Task Remove_Multiple_Links_Success()
	{
		const string domain = "Some domain";
		const string type = "Some type";
		Guid parserId = Guid.NewGuid();

		Result<SubscribedParser> subscribeResult = await Services.InvokeSubscription(domain, type, parserId);
		Assert.True(subscribeResult.IsSuccess);

		Result<IEnumerable<SubscribedParserLink>> link1Result = await Services.AddLink(
			parserId,
			"https://example1.com",
			"Link 1"
		);
		Assert.True(link1Result.IsSuccess);
		Guid link1Id = link1Result.Value.First().Id.Value;

		Result<IEnumerable<SubscribedParserLink>> link2Result = await Services.AddLink(
			parserId,
			"https://example2.com",
			"Link 2"
		);
		Assert.True(link2Result.IsSuccess);
		Guid link2Id = link2Result.Value.First().Id.Value;

		Result<IEnumerable<SubscribedParserLink>> link3Result = await Services.AddLink(
			parserId,
			"https://example3.com",
			"Link 3"
		);
		Assert.True(link3Result.IsSuccess);
		Guid link3Id = link3Result.Value.First().Id.Value;

		Result<SubscribedParserLink> remove1Result = await Services.RemoveLink(parserId, link1Id);
		Assert.True(remove1Result.IsSuccess);

		Result<SubscribedParserLink> remove2Result = await Services.RemoveLink(parserId, link2Id);
		Assert.True(remove2Result.IsSuccess);

		Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
		Assert.True(parserResult.IsSuccess);

		Result<SubscribedParserLink> findLink1 = parserResult.Value.FindLink(link1Id);
		Assert.True(findLink1.IsFailure);

		Result<SubscribedParserLink> findLink2 = parserResult.Value.FindLink(link2Id);
		Assert.True(findLink2.IsFailure);

		Result<SubscribedParserLink> findLink3 = parserResult.Value.FindLink(link3Id);
		Assert.True(findLink3.IsSuccess);
	}

	[Fact]
	private async Task Remove_Same_Link_Twice_Failure()
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

		Result<SubscribedParserLink> firstRemoveResult = await Services.RemoveLink(parserId, linkId);
		Assert.True(firstRemoveResult.IsSuccess);

		Result<SubscribedParserLink> secondRemoveResult = await Services.RemoveLink(parserId, linkId);
		Assert.True(secondRemoveResult.IsFailure);
	}

	[Fact]
	private async Task Remove_Link_And_Add_New_Link_With_Same_Name_Success()
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

		Result<SubscribedParserLink> removeResult = await Services.RemoveLink(parserId, linkId);
		Assert.True(removeResult.IsSuccess);

		Result<IEnumerable<SubscribedParserLink>> newLinkResult = await Services.AddLink(
			parserId,
			"https://newexample.com",
			"Test Link"
		);
		Assert.True(newLinkResult.IsSuccess);
		Assert.NotEqual(linkId, newLinkResult.Value.First().Id.Value);

		Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
		Assert.True(parserResult.IsSuccess);
		Result<SubscribedParserLink> findNewLink = parserResult.Value.FindLink(newLinkResult.Value.First().Id);
		Assert.True(findNewLink.IsSuccess);
	}

	[Fact]
	private async Task Remove_Link_And_Add_New_Link_With_Same_Url_Success()
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

		Result<SubscribedParserLink> removeResult = await Services.RemoveLink(parserId, linkId);
		Assert.True(removeResult.IsSuccess);

		Result<IEnumerable<SubscribedParserLink>> newLinkResult = await Services.AddLink(
			parserId,
			"https://example.com",
			"New Link Name"
		);
		Assert.True(newLinkResult.IsSuccess);
		Assert.NotEqual(linkId, newLinkResult.Value.First().Id.Value);

		Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
		Assert.True(parserResult.IsSuccess);
		Result<SubscribedParserLink> findNewLink = parserResult.Value.FindLink(newLinkResult.Value.First().Id);
		Assert.True(findNewLink.IsSuccess);
	}

	[Fact]
	private async Task Remove_All_Links_From_Parser_Success()
	{
		const string domain = "Some domain";
		const string type = "Some type";
		Guid parserId = Guid.NewGuid();

		Result<SubscribedParser> subscribeResult = await Services.InvokeSubscription(domain, type, parserId);
		Assert.True(subscribeResult.IsSuccess);

		Result<IEnumerable<SubscribedParserLink>> link1Result = await Services.AddLink(
			parserId,
			"https://example1.com",
			"Link 1"
		);
		Assert.True(link1Result.IsSuccess);

		Result<IEnumerable<SubscribedParserLink>> link2Result = await Services.AddLink(
			parserId,
			"https://example2.com",
			"Link 2"
		);
		Assert.True(link2Result.IsSuccess);

		Result<SubscribedParserLink> remove1Result = await Services.RemoveLink(
			parserId,
			link1Result.Value.First().Id.Value
		);
		Assert.True(remove1Result.IsSuccess);

		Result<SubscribedParserLink> remove2Result = await Services.RemoveLink(
			parserId,
			link2Result.Value.First().Id.Value
		);
		Assert.True(remove2Result.IsSuccess);

		Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
		Assert.True(parserResult.IsSuccess);
	}

	[Fact]
	private async Task Remove_Link_Preserves_Other_Link_Data_Success()
	{
		const string domain = "Some domain";
		const string type = "Some type";
		Guid parserId = Guid.NewGuid();

		Result<SubscribedParser> subscribeResult = await Services.InvokeSubscription(domain, type, parserId);
		Assert.True(subscribeResult.IsSuccess);

		Result<IEnumerable<SubscribedParserLink>> link1Result = await Services.AddLink(
			parserId,
			"https://example1.com",
			"Link 1"
		);
		Assert.True(link1Result.IsSuccess);
		Guid link1Id = link1Result.Value.First().Id.Value;

		Result<IEnumerable<SubscribedParserLink>> link2Result = await Services.AddLink(
			parserId,
			"https://example2.com",
			"Link 2"
		);
		Assert.True(link2Result.IsSuccess);
		Guid link2Id = link2Result.Value.First().Id.Value;

		Result<SubscribedParserLink> removeResult = await Services.RemoveLink(parserId, link1Id);
		Assert.True(removeResult.IsSuccess);

		Result<SubscribedParser> parserResult = await Services.GetParser(parserId);
		Assert.True(parserResult.IsSuccess);

		Result<SubscribedParserLink> remainingLink = parserResult.Value.FindLink(link2Id);
		Assert.True(remainingLink.IsSuccess);
	}
}
