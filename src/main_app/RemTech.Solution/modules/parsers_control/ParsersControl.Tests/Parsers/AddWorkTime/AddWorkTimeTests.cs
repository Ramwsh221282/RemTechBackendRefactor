using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.Features.SetWorkTime;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Tests.Parsers.AddWorkTime;

public sealed class AddWorkTimeTests(IntegrationalTestsFixture fixture) : IClassFixture<IntegrationalTestsFixture>
{
	private IServiceProvider Services { get; } = fixture.Services;

	[Fact]
	private async Task Add_Work_Time_When_Parser_Working_Success()
	{
		const string type = "Some Type";
		const string domain = "Some Domain";
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
		Result<SubscribedParser> enabled = await Services.EnableParser(id);
		Assert.True(enabled.IsSuccess);
		Result<SubscribedParser> started = await Services.StartParser(id);
		Assert.True(started.IsSuccess);
		Result<SubscribedParser> changed = await AddWorkTime(id, 10);
		Assert.True(changed.IsSuccess);
	}

	[Fact]
	private async Task Add_Work_Time_When_Not_Working_Failure()
	{
		const string type = "Some Type";
		const string domain = "Some Domain";
		Guid id = Guid.NewGuid();
		Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
		Assert.True(result.IsSuccess);
		Result<SubscribedParser> enabled = await Services.EnableParser(id);
		Assert.True(enabled.IsSuccess);
		Result<SubscribedParser> changed = await AddWorkTime(id, 10);
		Assert.False(changed.IsSuccess);
	}

	private async Task<Result<SubscribedParser>> AddWorkTime(Guid id, long totalElapsedSeconds)
	{
		SetWorkTimeCommand command = new(id, totalElapsedSeconds);
		await using AsyncServiceScope scope = Services.CreateAsyncScope();
		ICommandHandler<SetWorkTimeCommand, SubscribedParser> handler = scope.ServiceProvider.GetRequiredService<
			ICommandHandler<SetWorkTimeCommand, SubscribedParser>
		>();
		Result<SubscribedParser> changed = await handler.Execute(command);
		return changed;
	}
}
