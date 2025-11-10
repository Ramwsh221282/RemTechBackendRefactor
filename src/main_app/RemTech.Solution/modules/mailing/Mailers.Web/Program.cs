using Mailers.Web;
using SwaggerThemes;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddMailersModuleDependencies();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(Theme.UniversalDark);

app.ApplyMailersDbMigration();
app.MapControllers();

app.Run();