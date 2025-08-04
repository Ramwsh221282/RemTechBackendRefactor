using Mailing.Module.Injection;
using RemTech.Bootstrap.Api.Configuration;
using RemTech.Bootstrap.Api.Injection;
using RemTech.Vehicles.Module.Injection;
using Scalar.AspNetCore;
using Users.Module.Injection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
RemTechApplicationSettings settings = RemTechApplicationSettings.CreateFromJson("appsettings.json");
UsersModuleInjection.UpUsersModuleDatabase(settings.Database.ToConnectionString());
MailingModuleInjection.UpMailingModuleDatabase(settings.Database.ToConnectionString());
ParsedAdvertisementsInjection.UpVehiclesDatabase(settings.Database.ToConnectionString());
builder.Services.InjectCommonInfrastructure(settings);
builder.Services.InjectMailingModule();
builder.Services.InjectUsersModule();
builder.Services.InjectVehiclesModule();
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
builder.Services.AddOpenApi();
WebApplication app = builder.Build();
app.UseCors();
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
