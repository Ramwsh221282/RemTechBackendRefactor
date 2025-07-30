using RemTech.Logging.Adapter;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;
using RemTech.RabbitMq.Adapter;
using RemTech.Vehicles.Module.Injection;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
    options.AddPolicy("FRONTEND", conf => conf.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod())
);
builder.Services.AddSingleton(new DatabaseConfiguration("appsettings.json"));
builder.Services.AddSingleton(new RabbitMqConnectionOptions("appsettings.json"));
builder.Services.AddSingleton(new LoggerSource().Logger());
builder.Services.AddSingleton<PgConnectionSource>();
builder.Services.InjectVehiclesModule();
builder.Services.AddOpenApi();
WebApplication app = builder.Build();
app.UseCors();
app.Services.UpVehiclesDatabase();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseHttpsRedirection();
app.MapVehiclesModuleEndpoints();
app.Run();
