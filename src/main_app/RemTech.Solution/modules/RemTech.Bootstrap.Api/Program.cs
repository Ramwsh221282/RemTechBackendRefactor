using RemTech.Logging.Adapter;
using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Inject;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;
using RemTech.RabbitMq.Adapter;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(new DatabaseConfiguration("appsettings.json"));
builder.Services.AddSingleton(new RabbitMqConnectionOptions("appsettings.json"));
builder.Services.AddSingleton<ICustomLogger>(new ConsoleLogger());
builder.Services.AddSingleton<PgConnectionSource>();
builder.Services.InjectParsedAdvertisementsModule();
builder.Services.AddOpenApi();
WebApplication app = builder.Build();
app.Services.UpParsedAdvertisementsModuleDb().Wait();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Run();
