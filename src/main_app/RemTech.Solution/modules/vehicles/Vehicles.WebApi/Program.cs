using Serilog;
using SwaggerThemes;
using Vehicles.CompositionRoot;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Services.InjectVehiclesModule(AppDomain.CurrentDomain.GetAssemblies());
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

namespace Vehicles.Program
{
    public partial class Program { }
}
