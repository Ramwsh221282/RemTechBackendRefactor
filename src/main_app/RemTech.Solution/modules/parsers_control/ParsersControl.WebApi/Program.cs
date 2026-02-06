using ParsersControl.WebApi.Extensions;
using RemTech.SharedKernel.Infrastructure.Database;
using SwaggerThemes;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddParsersControlModule(builder.Environment.IsDevelopment());
WebApplication app = builder.Build();

if (args.Contains("--clean-migrations"))
{
	app.Services.RollBackModuleMigrations();
}

app.Services.ApplyModuleMigrations();
app.UseSwagger();
app.UseSwaggerUI(Theme.UniversalDark);
app.MapControllers();
app.Run();

namespace ParsersControl.WebApi
{
	public partial class Program { }
}
