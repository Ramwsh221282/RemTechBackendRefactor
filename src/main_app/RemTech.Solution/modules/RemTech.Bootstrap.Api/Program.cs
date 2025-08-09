using Mailing.Module.Injection;
using Quartz;
using RemTech.Bootstrap.Api.Configuration;
using RemTech.Bootstrap.Api.Injection;
using RemTech.Vehicles.Module.Injection;
using RemTech.Vehicles.Module.OnStartup;
using Scalar.AspNetCore;
using Scrapers.Module.Inject;
using Users.Module.Injection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
RemTechApplicationSettings settings = RemTechApplicationSettings.CreateFromJson("appsettings.json");
UsersModuleInjection.UpUsersModuleDatabase(settings.Database.ToConnectionString());
MailingModuleInjection.UpMailingModuleDatabase(settings.Database.ToConnectionString());
ParsedAdvertisementsInjection.UpVehiclesDatabase(settings.Database.ToConnectionString());
ScrapersModuleInjection.UpScrapersModuleDatabase(settings.Database.ToConnectionString());
builder.Services.InjectCommonInfrastructure(settings);
builder.Services.InjectScrapersModule();
builder.Services.InjectMailingModule();
builder.Services.InjectUsersModule();
builder.Services.InjectVehiclesModule();
builder.Services.AddCors(options =>
    options.AddPolicy(
        "FRONTEND",
        conf => conf.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()
    )
);
builder.Services.AddOpenApi();
builder.Services.AddQuartzHostedService();
WebApplication app = builder.Build();
CreateVectorsOnStartup createVectors = app.Services.GetRequiredService<CreateVectorsOnStartup>();
createVectors.Create().Wait();
app.UseCors("FRONTEND");
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseHttpsRedirection();
app.MapMailingModuleEndpoints();
app.MapVehiclesModuleEndpoints();
app.MapUsersModuleEndpoints();
app.MapScrapersModuleEndpoints();
app.Run();
