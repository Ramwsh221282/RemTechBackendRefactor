using SwaggerThemes;
using Vehicles.CompositionRoot;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.InjectVehiclesModule(AppDomain.CurrentDomain.GetAssemblies());
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

namespace Vehicles.WebApi
{
    public partial class Program { }
}
