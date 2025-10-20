using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Quartz;
using RemTech.Bootstrap.Api.Configuration;
using RemTech.ContainedItems.Module.Grpc;
using RemTech.Shared.Configuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
new ConfigurationPartsBuilder(builder.Services).AddConfigurations();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(
        IPAddress.Any,
        5185,
        opts =>
        {
            opts.Protocols = HttpProtocols.Http1AndHttp2;
        }
    );
    options.Listen(
        IPAddress.Any,
        5238,
        opts =>
        {
            opts.Protocols = HttpProtocols.Http2;
        }
    );
});

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
