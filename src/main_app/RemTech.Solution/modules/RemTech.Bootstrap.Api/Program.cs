using Brands.Module.Injection;
using Categories.Module.Injection;
using GeoLocations.Module.Injection;
using Mailing.Module.Injection;
using Models.Module.Injection;
using Quartz;
using RemTech.Bootstrap.Api.Configuration;
using RemTech.Bootstrap.Api.Injection;
using RemTech.ContainedItems.Module.Injection;
using RemTech.Spares.Module.Injection;
using RemTech.Vehicles.Module.Injection;
using RemTech.Vehicles.Module.OnStartup;
using Scalar.AspNetCore;
using Scrapers.Module.Inject;
using Users.Module.Injection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

RemTechApplicationSettings settings = RemTechApplicationSettings.CreateFromJson("appsettings.json");
UsersModuleInjection.UpDatabase(settings.Database.ToConnectionString());
MailingModuleInjection.UpDatabase(settings.Database.ToConnectionString());
ParsedAdvertisementsInjection.UpDatabase(settings.Database.ToConnectionString());
ScrapersModuleInjection.UpDatabase(settings.Database.ToConnectionString());
SparesModuleInjection.UpDatabase(settings.Database.ToConnectionString());
GeoLocationsModuleInjection.UpDatabase(settings.Database.ToConnectionString());
BrandsModuleInjection.UpDatabase(settings.Database.ToConnectionString());
CategoriesModuleInjection.UpDatabase(settings.Database.ToConnectionString());
ModelsModuleInjection.UpDatabase(settings.Database.ToConnectionString());
ContainedItemsModuleInjection.UpDatabase(settings.Database.ToConnectionString());

builder.Services.InjectCommonInfrastructure(settings);
builder.Services.InjectScrapersModule();
builder.Services.InjectMailingModule();
builder.Services.InjectUsersModule();
builder.Services.InjectVehiclesModule();
builder.Services.InjectSparesModule();
builder.Services.InjectLocationsModule();
builder.Services.InjectBrandsModule();
builder.Services.InjectCategoriesModule();
builder.Services.InjectModelsModule();
builder.Services.InjectContainedItemsModule();

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
app.MapSparesEndpoints();

app.Run();
