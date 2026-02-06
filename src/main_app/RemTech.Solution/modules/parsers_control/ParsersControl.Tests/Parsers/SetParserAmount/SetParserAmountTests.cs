using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.Features.SetParsedAmount;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Tests.Parsers.SetParserAmount;

/// <summary>
///    Тесты для установки количества распарсенных элементов парсера.
/// </summary>
/// <param name="fixture">Фикстура для интеграционных тестов.</param>
public sealed class SetParserAmountTests(IntegrationalTestsFixture fixture) : IClassFixture<IntegrationalTestsFixture>
{
	private IServiceProvider Services { get; } = fixture.Services;

	/// <summary>
	///   Установка количества распарсенных элементов парсера успешно.
	/// </summary>
	/// <returns> Усановить количество распарсенных элементов парсера успешно.</returns>
	[Fact]
	public async Task Set_Parser_Amount_Success()
	{
		const string type = "Some Type";
		const string domain = "Some Domain";
		Guid id = Guid.NewGuid();
		Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
		Assert.True(result.IsSuccess);
		Result<SubscribedParser> enabled = await Services.EnableParser(id);
		Assert.True(enabled.IsSuccess);
		Result<SubscribedParser> started = await Services.StartParser(id);
		Assert.True(started.IsSuccess);
		Result<SubscribedParser> changed = await SetParserAmount(id, 10);
		Assert.True(changed.IsSuccess);
	}

	[Fact]
	private async Task Set_Parser_Amount_When_Not_Working_Failure()
	{
		const string type = "Some Type";
		const string domain = "Some Domain";
		Guid id = Guid.NewGuid();
		Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
		Assert.True(result.IsSuccess);
		Result<SubscribedParser> enabled = await Services.EnableParser(id);
		Assert.True(enabled.IsSuccess);
		Result<SubscribedParser> changed = await SetParserAmount(id, 10);
		Assert.False(changed.IsSuccess);
	}

	private async Task<Result<SubscribedParser>> SetParserAmount(Guid id, int amount)
	{
		SetParsedAmountCommand command = new(id, amount);
		await using AsyncServiceScope scope = Services.CreateAsyncScope();
		ICommandHandler<SetParsedAmountCommand, SubscribedParser> handler = scope.ServiceProvider.GetRequiredService<
			ICommandHandler<SetParsedAmountCommand, SubscribedParser>
		>();
		Result<SubscribedParser> changed = await handler.Execute(command);
		return changed;
	}
}
