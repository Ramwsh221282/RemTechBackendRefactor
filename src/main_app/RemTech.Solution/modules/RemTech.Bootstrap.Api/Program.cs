using Mailing.Module.Configuration;
using Mailing.Module.Injection;
using RemTech.Logging.Adapter;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;
using RemTech.RabbitMq.Adapter;
using RemTech.Vehicles.Module.Injection;
using Scalar.AspNetCore;
using StackExchange.Redis;
using Users.Module.Injection;
using Users.Module.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
MailingModuleOptions mailingOptions = new("mailing_module.json");
UsersModuleOptions userOptions = new("users_module.json");
builder.Services.InjectMailingModule(mailingOptions);
builder.Services.InjectUsersModule(userOptions);
builder.Services.AddCors(options =>
    options.AddPolicy(
        "FRONTEND",
        conf =>
            conf.WithExposedHeaders("Authorization")
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod()
    )
);

builder.Services.AddSingleton(ConnectionMultiplexer.Connect("localhost"));

builder.Services.AddSingleton(new DatabaseConfiguration("appsettings.json"));
builder.Services.AddSingleton(new RabbitMqConnectionOptions("appsettings.json"));
builder.Services.AddSingleton(new LoggerSource().Logger());
builder.Services.AddSingleton<PgConnectionSource>();
builder.Services.InjectVehiclesModule();
builder.Services.AddOpenApi();
WebApplication app = builder.Build();
app.UseCors();
app.Services.UpVehiclesDatabase();
userOptions.UpUsersModulePersistance();
mailingOptions.UpMailingModuleDatabase();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseHttpsRedirection();
app.MapMailingModuleEndpoints();
app.MapVehiclesModuleEndpoints();
app.MapUsersModuleEndpoints();
app.Run();
