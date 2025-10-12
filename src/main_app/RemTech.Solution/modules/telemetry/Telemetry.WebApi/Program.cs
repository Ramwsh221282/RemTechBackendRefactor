using Remtech.Infrastructure.RabbitMQ;
using Remtech.Infrastructure.RabbitMQ.Consumers;
using Serilog;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using SwaggerThemes;
using Telemetry.CompositionRoot;
using Telemetry.Infrastructure.PostgreSQL;
using Telemetry.Infrastructure.PostgreSQL.Repositories;
using Telemetry.WebApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterFromJsonRoot<PostgreSqlConnectionOptions>();
builder.RegisterFromJsonRoot<RabbitMqOptions>();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
OnnxEmbeddingGenerator.AddEmbeddingGenerator(builder.Services);
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
