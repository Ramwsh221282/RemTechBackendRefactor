using ParserSubscriber.SubscribtionContext;
using Quartz;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.Quartz;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTechAvitoVehiclesParser;
using RemTechAvitoVehiclesParser.RabbitMq.Consumers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
bool isDevelopment = builder.Environment.IsDevelopment();

builder.Services.RegisterLogging();
builder.Services.RegisterDependenciesForParsing(isDevelopment);
builder.Services.RegisterInfrastructureDependencies(isDevelopment);
builder.Services.AddCronScheduledJobs();
builder.Services.AddTransient<IConsumer, ParserStartConsumer>();
builder.Services.AddHostedService<AggregatedConsumersHostedService>();
builder.Services.AddQuartzHostedService(c =>
{
    c.WaitForJobsToComplete = true;
    c.StartDelay = TimeSpan.FromSeconds(10);
});

WebApplication app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    _ = Task.Run(async () =>
    {
        await app.Services.RunParserSubscription();
    });
});

if (args.Contains("--rollback-migrations"))
{
    app.Services.RollBackModuleMigrations();
}

app.Services.ApplyModuleMigrations();
app.Run();

namespace RemTechAvitoVehiclesParser
{
    public partial class Program { }
}
