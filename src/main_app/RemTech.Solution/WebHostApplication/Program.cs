using System.Text.Encodings.Web;
using System.Text.Unicode;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using Serilog;
using Serilog.Core;
using SwaggerThemes;
using WebHostApplication.Injection;
using WebHostApplication.Middlewares;
using WebHostApplication.Middlewares.Telemetry;

// TODO: Add rate limiters.
// TODO: Add response compression.

Logger logger = new LoggerConfiguration().Enrich.With(new ClassNameLogEnricher()).WriteTo.Console().CreateLogger();
logger.Information("Запуск веб-приложения.");

try
{
	WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
	builder.Configuration.Register(builder.Services, builder.Environment.IsDevelopment());
	logger.Information("Конфигурация зарегистрирована.");

	builder.Services.RegisterSharedDependencies(builder.Configuration);
	logger.Information("Общие зависимости зарегистрированы.");

	builder.Services.RegisterApplicationModules();
	logger.Information("Модули приложения зарегистрированы.");

	builder.Services.RegisterModuleMigrations();
	logger.Information("Миграции модулей зарегистрированы.");

	builder.Services.AddSwaggerGen();

	builder
		.Services.AddControllers()
		.AddJsonOptions(options => options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All));
	builder.Services.AddEndpointsApiExplorer();
	logger.Information("Контроллеры зарегистрированы.");

	builder.Services.AddCors(options =>
	{
		IConfigurationSection section = builder.Configuration.GetSection(nameof(FrontendOptions));
		string? url = section["Url"] ?? throw new InvalidOperationException("Frontend URL option is empty.");
		options.AddPolicy(
			"frontend",
			policy => policy.WithOrigins(url).AllowCredentials().AllowAnyMethod().AllowAnyHeader()
		);
	});
	logger.Information("CORS политика зарегистрирована.");

	logger.Information("Построение приложения...");
	WebApplication app = builder.Build();

	await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
	IServiceProvider sp = scope.ServiceProvider;

	logger.Information("Применение миграций модулей...");
	app.Services.ApplyModuleMigrations();

	app.UseHttpsRedirection();

	logger.Information("Настройка middleware...");
	app.UseCors("frontend");
	app.MapControllers();
	app.UseSwagger();

	app.UseSwaggerUI(Theme.UniversalDark);
	app.UseMiddleware<ExceptionMiddleware>();
	app.UseMiddleware<TelemetryRecordWritingMiddleware>();

	logger.Information("Запуск приложения...");
	app.Run();
}
catch (Exception ex)
{
	logger.Fatal(ex, "Host terminated unexpectedly.");
	return;
}
finally
{
	logger.Information("Завершение работы логгера старта...");
	await logger.DisposeAsync();
}

namespace WebHostApplication
{
	/// <summary>
	/// Главная точка входа для приложения WebHostApplication (этот нужен для тестов).
	/// </summary>
	public partial class Program { }
}
