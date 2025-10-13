using RemTech.DependencyInjection;
using RemTech.Infrastructure.PostgreSQL;
using Remtech.Infrastructure.RabbitMQ;
using Serilog;
using SwaggerThemes;
using Telemetry.CompositionRoot;
using Telemetry.Infrastructure.PostgreSQL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterFromJsonRoot<RabbitMqOptions>();
builder.RegisterFromJsonRoot<NpgsqlOptions>();

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Services.AddRabbitMqProvider();
builder.Services.AddScoped<TelemetryServiceDbContext>();
builder.Services.InjectTelemetryModule(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton(logger);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(Theme.UniversalDark);
    app.MapOpenApi();
}

app.MapControllers();
app.MapSwagger();
app.Run();

namespace Telemetry.WebApi
{
    public partial class Program { }
}
