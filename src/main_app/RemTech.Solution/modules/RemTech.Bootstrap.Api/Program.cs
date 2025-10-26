using Identity.WebApi;
using Quartz;
using RemTech.Bootstrap.Api.Configuration;
using RemTech.Bootstrap.Api.Extensions;
using RemTech.ContainedItems.Module.Grpc;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddConfigurations();
builder.ConfigureWebHost(5185, 5238);

builder.Services.AddIdentityModule();

builder.Services.InjectModules();
builder.Services.ConfigureQuartz();
builder.ConfigureCors();
builder.Services.AddGrpc();
builder.Services.AddOpenApi();
builder.Services.AddQuartzHostedService();

WebApplication app = builder.Build();
await app.UpDatabases();

app.RegisterMiddlewares();
app.MapModulesEndpoints();
app.MapGrpcService<DuplicateIdsCheckService>();
app.Run();
