using System.Text.Encodings.Web;
using System.Text.Unicode;
using Identity.Domain.Accounts.Features.Dev_ChangePassword;
using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Web;
using SwaggerThemes;
using Telemetry.Infrastructure;
using WebHostApplication.ActionFilters.Filters.TelemetryFilters;
using WebHostApplication.Injection;
using WebHostApplication.Middlewares;

// TODO: Add rate limiters.
// TODO: Add response compression.

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterApplicationModules();
if (builder.Environment.IsDevelopment())
{
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

// TODO: move to dependency injection method such as inject shared dependencies.
builder.Services.AddSingleton<TelemetryRecordInvokerIdSearcher>();
builder.Services.RegisterRedisActionRecordDependencies();

WebApplication app = builder.Build();

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
	/// Главная точка входа для приложения WebHostApplication.
	/// </summary>
	public partial class Program { }
}
