using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Tests.Parsers.ChangeLinkActivity;

/// <summary>
///    Тесты для изменения активности ссылки на парсер.
/// </summary>
/// <param name="fixture">Фикстура для интеграционных тестов.</param>
public sealed class ChangeLinkActivityTests(IntegrationalTestsFixture fixture)
	: IClassFixture<IntegrationalTestsFixture>
{
	private IServiceProvider Services { get; } = fixture.Services;

	[Fact]
	private async Task Change_Link_Activity_Success()
	{
		const string type = "Some type";
		const string domain = "Some domain";
		Guid id = Guid.NewGuid();
		Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
		Assert.True(result.IsSuccess);
		Result<IEnumerable<SubscribedParserLink>> linkResult = await Services.AddLink(id, "Some url", "Some name");
		Assert.True(linkResult.IsSuccess);
		Result<SubscribedParserLink> activityResult = await Services.ChangeLinkActivity(
			id,
			linkResult.Value.First().Id.Value,
			true
		);
		Assert.True(activityResult.IsSuccess);

		Result<SubscribedParser> parser = await Services.GetParser(id);
		Assert.True(parser.IsSuccess);
		Result<SubscribedParserLink> link = parser.Value.FindLink(linkResult.Value.First().Id);
		Assert.True(link.IsSuccess);
		Assert.True(link.Value.Active);
	}

	[Fact]
	private async Task Change_Link_Activity_When_Parser_Working_Failure()
	{
		const string type = "Some type";
		const string domain = "Some domain";
		Guid id = Guid.NewGuid();
		Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
		Assert.True(result.IsSuccess);
		Result<IEnumerable<SubscribedParserLink>> linkResultBeforeStartWork = await Services.AddLink(
			id,
			"Test url",
			"Test name"
		);
		Assert.True(linkResultBeforeStartWork.IsSuccess);
		Result activatingLink = await Services.MakeLinkActive(id, linkResultBeforeStartWork.Value.First().Id.Value);
		Assert.True(activatingLink.IsSuccess);
		Result<SubscribedParser> enableResult = await Services.EnableParser(id);
		Assert.True(enableResult.IsSuccess);
		Result<SubscribedParser> startResult = await Services.StartParser(id);
		Assert.True(startResult.IsSuccess);
		Result<SubscribedParserLink> activityResult = await Services.ChangeLinkActivity(
			id,
			linkResultBeforeStartWork.Value.First().Id.Value,
			false
		);
		Assert.True(activityResult.IsFailure);
	}
}
