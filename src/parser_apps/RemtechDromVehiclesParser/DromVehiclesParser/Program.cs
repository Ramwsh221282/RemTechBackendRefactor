using DromVehiclesParser.DependencyInjection;
using ParserSubscriber.SubscribtionContext;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
bool isDevelopment = builder.Environment.IsDevelopment();
builder.Services.RegisterLogging();
builder.Services.RegisterParserSubscription();
builder.Services.RegisterDromParserStartQueue();
builder.Services.RegisterDependenciesForParsing(isDevelopment);
builder.Services.RegisterInfrastructureDependencies(isDevelopment);

WebApplication app = builder.Build();
app.Services.ApplyModuleMigrations();
await app.Services.RunParserSubscription();
app.Run();

namespace DromVehiclesParser
{
    public partial class Program
    {
        
    }
}