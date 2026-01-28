using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace RemTech.SharedKernel.Core.Logging;

/// <summary>
/// Расширения для регистрации логгера в контейнере служб.
/// </summary>
public static class LoggingInjection
{
	extension(IServiceCollection services)
	{
		public void RegisterLogging()
		{
			ILogger logger = new LoggerConfiguration()
				.Enrich.With(new ClassNameLogEnricher())
				.WriteTo.Console(
					outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message}{NewLine}{Exception}"
				)
				.CreateLogger();
			services.AddSingleton(logger);
		}
	}
}
