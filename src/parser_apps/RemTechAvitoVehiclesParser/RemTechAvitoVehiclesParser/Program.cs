using ParserSubscriber.SubscribtionContext;
using Quartz;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.Quartz;
using RemTechAvitoVehiclesParser;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
bool isDevelopment = builder.Environment.IsDevelopment();

builder.Services.RegisterLogging();
builder.Services.RegisterDependenciesForParsing(isDevelopment);
builder.Services.RegisterAvitoStartQueue();
builder.Services.RegisterInfrastructureDependencies(isDevelopment);
builder.Services.AddCronScheduledJobs();
builder.Services.AddQuartzHostedService(c =>
{
    c.WaitForJobsToComplete = true;
    c.StartDelay = TimeSpan.FromSeconds(10);
});

WebApplication app = builder.Build();
if (args.Contains("--rollback-migrations"))
{
    app.Services.RollBackModuleMigrations();
}

app.Services.ApplyModuleMigrations();
await app.Services.RunParserSubscription();
app.Run();

namespace RemTechAvitoVehiclesParser
{
    public partial class Program { }
}
