using Quartz;
using RemTech.Bootstrap.Api.Configuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
RemTechApplicationSettings settings = RemTechApplicationSettings.ResolveByEnvironment(builder);
settings.UpDatabases();
builder.Services.InjectModules(settings);
builder.Services.ConfigureQuartz();
settings.ConfigureCors(builder);
builder.Services.AddOpenApi();
builder.Services.AddQuartzHostedService();
WebApplication app = builder.Build();
app.RegisterMiddlewares();
app.MapModulesEndpoints();
app.Run();
