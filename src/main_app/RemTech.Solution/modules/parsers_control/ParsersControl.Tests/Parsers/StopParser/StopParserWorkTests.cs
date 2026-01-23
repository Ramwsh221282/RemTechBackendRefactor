using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Tests.Parsers.StopParser;

public sealed class StopParserWorkTests(IntegrationalTestsFixture fixture) : IClassFixture<IntegrationalTestsFixture>
{
	private IServiceProvider Services { get; } = fixture.Services;

	[Fact]
	private async Task Stop_Parser_Work_From_Working_State_Failure()
	{
		const string domain = "Some Domain";
		const string type = "Some Type";
		Guid id = Guid.NewGuid();
		Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
		Assert.True(result.IsSuccess);
		Result<SubscribedParser> enabled = await Services.EnableParser(id);
		Assert.True(enabled.IsSuccess);
		Result<SubscribedParser> started = await Services.StartParser(id);
		Assert.True(started.IsSuccess);
		Result<SubscribedParser> disabled = await Services.DisableParser(id);
		Assert.True(disabled.IsSuccess);
	}

	[Fact]
	private async Task Stop_Parser_Work_After_Creation_With_Disabled_State_Failure()
	{
		const string domain = "Some Domain";
		const string type = "Some Type";
		Guid id = Guid.NewGuid();
		Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
		Assert.True(result.IsSuccess);
		Result<SubscribedParser> disabled = await Services.DisableParser(id);
		Assert.True(disabled.IsFailure);
	}

	[Fact]
	private async Task Stop_Parser_Work_At_Sleeping_State_Failure()
	{
		const string domain = "Some Domain";
		const string type = "Some Type";
		Guid id = Guid.NewGuid();
		Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
		Assert.True(result.IsSuccess);
		Result<SubscribedParser> sleep = await Services.SleepParser(id);
		Assert.True(sleep.IsFailure);
		Result<SubscribedParser> disabled = await Services.DisableParser(id);
		Assert.True(disabled.IsFailure);
	}
}
