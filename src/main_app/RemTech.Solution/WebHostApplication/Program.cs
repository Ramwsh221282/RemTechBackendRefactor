using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Infrastructure.Database;
using SwaggerThemes;
using WebHostApplication.Injection;
using WebHostApplication.Middlewares;
using WebHostApplication.Middlewares.Telemetry;

// TODO: Add rate limiters.
// TODO: Add response compression.

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterApplicationModules();
if (builder.Environment.IsDevelopment())
{
	builder.InvokeTestEnv();
	builder.Services.RegisterConfigurationFromAppsettings();
}

builder.Services.RegisterSharedDependencies(builder.Configuration);
builder.Services.RegisterModuleMigrations();
builder.Services.AddSwaggerGen();
builder
	.Services.AddControllers()
	.AddJsonOptions(options => options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
	IConfigurationSection section = builder.Configuration.GetSection(nameof(FrontendOptions));
	string? url = section["Url"] ?? throw new InvalidOperationException("Frontend URL option is empty.");
	options.AddPolicy(
		"frontend",
		policy => policy.WithOrigins(url).AllowCredentials().AllowAnyMethod().AllowAnyHeader()
	);
});

WebApplication app = builder.Build();

await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
IServiceProvider sp = scope.ServiceProvider;
IOptions<TestEnv> env = sp.GetRequiredService<IOptions<TestEnv>>();

app.Services.ApplyModuleMigrations();
app.UseHttpsRedirection();
app.UseCors("frontend");
app.MapControllers();
app.UseSwagger();

app.UseSwaggerUI(Theme.UniversalDark);

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TelemetryRecordWritingMiddleware>();

app.Run();

namespace WebHostApplication
{
	/// <summary>
	/// Главная точка входа для приложения WebHostApplication (этот нужен для тестов).
	/// </summary>
	public partial class Program { }
}
