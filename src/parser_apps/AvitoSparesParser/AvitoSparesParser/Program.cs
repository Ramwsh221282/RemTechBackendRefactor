using AvitoSparesParser.Extensions;
using AvitoSparesParser.RabbitMq.Consumers;
using ParserSubscriber.SubscribtionContext;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
bool isDevelopment = builder.Environment.IsDevelopment();

builder.Services.RegisterLogging();
builder.Services.RegisterDependenciesForParsing(isDevelopment);
builder.Services.AddTransient<IConsumer, StartParserConsumer>();
builder.Services.AddHostedService<AggregatedConsumersHostedService>();
builder.Services.RegisterInfrastructureDependencies(isDevelopment);
WebApplication app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    _ = Task.Run(async () =>
    {
        await app.Services.RunParserSubscription();
    });
});

app.Services.ApplyModuleMigrations();
app.Run();

namespace AvitoSparesParser
{
    public partial class Program { }
}
