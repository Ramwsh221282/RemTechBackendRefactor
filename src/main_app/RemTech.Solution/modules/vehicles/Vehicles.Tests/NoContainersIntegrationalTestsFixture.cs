using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Configurations;

namespace Vehicles.Tests;

/// <summary>
/// Фикстура для интеграционных тестов без использования контейнеров.
/// </summary>
public sealed class NoContainersIntegrationalTestsFixture : WebApplicationFactory<Vehicles.WebApi.Program>
{
	/// <summary>
	/// Настраивает веб-хост для использования пользовательских настроек в тестах.
	/// </summary>
	/// <param name="builder">Веб-хост билдер для настройки.</param>
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		base.ConfigureWebHost(builder);
		builder.ConfigureServices(ReconfigureEmbeddingModelPath);
	}

	private void ReconfigureEmbeddingModelPath(IServiceCollection services)
	{
		services.RemoveAll<IConfigureOptions<EmbeddingsProviderOptions>>();
		services.RemoveAll<IOptions<EmbeddingsProviderOptions>>();
		string basePath = AppDomain.CurrentDomain.BaseDirectory;
		string onnxFolder = Path.Combine(basePath, "onnx");
		string tokenizerPath = Path.Combine(onnxFolder, "tokenizer.onnx");
		string modelPath = Path.Combine(onnxFolder, "model.onnx");
		EmbeddingsProviderOptions options = new() { TokenizerPath = tokenizerPath, ModelPath = modelPath };
		IOptions<EmbeddingsProviderOptions> ioptions = Options.Create(options);
		services.AddSingleton(ioptions);
	}
}
