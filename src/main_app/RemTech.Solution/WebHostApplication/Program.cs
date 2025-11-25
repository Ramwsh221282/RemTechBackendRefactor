using CompositionRoot.Shared;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterSharedServices();
builder.Services.RegisterModules();

WebApplication app = builder.Build();
app.Services.ApplyModuleMigrations();

app.Run();

namespace WebHostApplication
{
    public partial class Program { }
}