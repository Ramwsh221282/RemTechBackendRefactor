using ParsersControl.CompositionRoot;
using ParsersControl.Infrastructure.Migrations;
using ParsersControl.WebApi.Extensions;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using SwaggerThemes;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterLogging();
builder.Services.AddInfrastructureConfiguration();
builder.Services.AddPostgres();
builder.Services.AddRabbitMq();
builder.Services.AddMigrations([typeof(ParsersTableMigration).Assembly]);
builder.Services.AddParsersControlModule();

WebApplication app = builder.Build();
app.Services.ApplyModuleMigrations();
app.Services.RollBackModuleMigrations();

app.UseSwagger();
app.UseSwaggerUI(Theme.UniversalDark);
app.MapControllers();

app.Run();

namespace ParsersControl.WebApi
{
    public partial class Program
    {
        
    }
}