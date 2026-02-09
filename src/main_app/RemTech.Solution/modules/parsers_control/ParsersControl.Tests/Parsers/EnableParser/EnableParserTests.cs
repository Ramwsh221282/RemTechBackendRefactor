using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Tests.Parsers.EnableParser;

/// <summary>
///    Тесты для включения парсера.
/// </summary>
/// <param name="fixture">Фикстура для интеграционных тестов.</param>
public sealed class EnableParserTests(IntegrationalTestsFixture fixture) : IClassFixture<IntegrationalTestsFixture>
{
	private IServiceProvider Services { get; } = fixture.Services;

	/// <summary>
	///     Включение парсера успешно.
	/// </summary>
	/// <returns>Task representing the asynchronous operation.</returns>
	[Fact]
	public async Task Enable_Parser_Success()
	{
		const string type = "Some Type";
		const string domain = "Some Domain";
		Guid id = Guid.NewGuid();
		Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
		Assert.True(result.IsSuccess);
		Result<SubscribedParser> enabled = await Services.EnableParser(id);
		Assert.True(enabled.IsSuccess);
	}

	[Fact]
	private async Task Enable_Parser_When_Already_Enabled_Failure()
	{
		const string type = "Some Type";
		const string domain = "Some Domain";
		Guid id = Guid.NewGuid();
		Result<SubscribedParser> result = await Services.InvokeSubscription(domain, type, id);
		Assert.True(result.IsSuccess);
		Result<SubscribedParser> enabled = await Services.EnableParser(id);
		Assert.True(enabled.IsSuccess);
		Result<SubscribedParser> again = await Services.EnableParser(id);
		Assert.False(again.IsSuccess);
	}
}
