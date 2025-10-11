using Serilog;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using Telemetry.CompositionRoot;
using Telemetry.Infrastructure.PostgreSQL;
using Telemetry.Infrastructure.PostgreSQL.Repositories;

var builder = WebApplication.CreateBuilder(args);

Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

PostgreSqlConnectionOptions? options = builder
    .Configuration.GetSection(nameof(PostgreSqlConnectionOptions))
    .Get<PostgreSqlConnectionOptions>();

if (options == null)
    throw new ArgumentException("Postgre sql connection options not set.");

OnnxEmbeddingGenerator.AddEmbeddingGenerator(builder.Services);
builder.Services.AddSingleton(options);
builder.Services.AddScoped<TelemetryServiceDbContext>();
builder.Services.InjectTelemetryModule(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton(logger);

WebApplication app = builder.Build();

app.Run();

namespace Telemetry.WebApi
{
    public partial class Program { }
}
