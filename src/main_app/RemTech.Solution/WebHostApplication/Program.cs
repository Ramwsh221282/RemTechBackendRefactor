using CompositionRoot.Shared;
using Quartz;
using RemTech.NpgSql.Abstractions;
using RemTech.RabbitMq.Abstractions;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Services.AddSingleton(logger);
builder.Services.AddPostgres();
builder.Services.AddRabbitMq();
builder.Services.RegisterModules();
builder.Services.AddQuartzHostedService(c => c.StartDelay = TimeSpan.FromSeconds(15));

WebApplication app = builder.Build();

app.Services.ApplyModuleMigrations();

app.Run();

namespace WebHostApplication
{
    public partial class Program { }
}