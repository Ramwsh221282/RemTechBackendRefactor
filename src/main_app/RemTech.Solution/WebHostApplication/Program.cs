using CompositionRoot.Shared;
using SwaggerThemes;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.RegisterSharedServices();
builder.Services.RegisterModules();
builder.Services.AddQuartzJobs();

WebApplication app = builder.Build();
app.Services.ApplyModuleMigrations();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI(Theme.UniversalDark);

app.Run();

namespace WebHostApplication
{
    public partial class Program { }
}