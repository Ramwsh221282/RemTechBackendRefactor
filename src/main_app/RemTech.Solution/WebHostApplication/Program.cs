using RemTech.SharedKernel.Infrastructure.Database;
using SwaggerThemes;
using WebHostApplication.Injection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterApplicationModules();
if (builder.Environment.IsDevelopment()) 
    builder.Services.RegisterConfigurationFromAppsettings();
builder.Services.RegisterSharedDependencies();
builder.Services.RegisterModuleMigrations();

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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