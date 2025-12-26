using DromVehiclesParser.DependencyInjection;
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
app.Run();

namespace DromVehiclesParser
{
    public partial class Program
    {
        
    }
}