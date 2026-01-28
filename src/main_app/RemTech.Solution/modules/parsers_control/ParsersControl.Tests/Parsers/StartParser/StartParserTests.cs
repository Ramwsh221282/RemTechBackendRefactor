using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Tests.Parsers.StartParser;

/// <summary>
///   Тесты для запуска парсера.
/// </summary>
/// <param name="fixture">Фикстура для интеграционных тестов.</param>
public sealed class StartParserTests(IntegrationalTestsFixture fixture) : IClassFixture<IntegrationalTestsFixture>
{
	private IServiceProvider Services { get; } = fixture.Services;

	/// <summary>
	///    Запуск парсера после его включения успешен.
	/// </summary>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	[Fact]
	public async Task Start_After_Disabled_Success()
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
	}

	/// <summary>
	///   Запуск отключенного парсера приводит к ошибке.
	/// </summary>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	[Fact]
	public async Task Start_Parser_Disabled_Failure()
	{
		const string type = "Some Type";
		const string domain = "Some Domain";
		Guid id = Guid.NewGuid();
		Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
		Assert.True(result.IsSuccess);
		Result<SubscribedParser> started = await Services.StartParser(id);
		Assert.True(started.IsFailure);
	}
}
