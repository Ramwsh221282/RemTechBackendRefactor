using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.Features.ChangeSchedule;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Tests.Parsers.ChangeSchedule;

public sealed class ChangeScheduleTests(IntegrationalTestsFixture fixture) : IClassFixture<IntegrationalTestsFixture>
{
	private IServiceProvider Services { get; } = fixture.Services;

	[Fact]
	public async Task Change_Schedule_Success()
	{
		const string type = "Some Type";
		const string domain = "Some Domain";
		Guid id = Guid.NewGuid();
		Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
		Assert.True(result.IsSuccess);
		Result<SubscribedParser> enabled = await Services.EnableParser(id);
		Assert.True(enabled.IsSuccess);
		Result<SubscribedParser> changed = await ChangeSchedule(id);
		Assert.True(changed.IsSuccess);
	}

	[Fact]
	public async Task Change_Schedule_When_Working_Failure()
	{
		const string type = "Some Type";
		const string domain = "Some Domain";
		Guid id = Guid.NewGuid();
		Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
		Assert.True(result.IsSuccess);
		Result<SubscribedParser> enabled = await Services.EnableParser(id);
		Assert.True(enabled.IsSuccess);
		Result<IEnumerable<SubscribedParserLink>> linkResultBeforeStartWork = await Services.AddLink(
			id,
			"Test url",
			"Test name"
		);
		Assert.True(linkResultBeforeStartWork.IsSuccess);
		Result activatingLink = await Services.MakeLinkActive(id, linkResultBeforeStartWork.Value.First().Id.Value);
		Assert.True(activatingLink.IsSuccess);
		Result<SubscribedParser> started = await Services.StartParser(id);
		Assert.True(started.IsSuccess);
		Result<SubscribedParser> changed = await ChangeSchedule(id);
		Assert.False(changed.IsSuccess);
	}

	private async Task<Result<SubscribedParser>> ChangeSchedule(Guid id)
	{
		ChangeScheduleCommand command = new(id, 1, DateTime.Now.AddDays(1));
		await using AsyncServiceScope scope = Services.CreateAsyncScope();
		ICommandHandler<ChangeScheduleCommand, SubscribedParser> handler = scope.ServiceProvider.GetRequiredService<
			ICommandHandler<ChangeScheduleCommand, SubscribedParser>
		>();
		Result<SubscribedParser> changed = await handler.Execute(command);
		return changed;
	}
}
