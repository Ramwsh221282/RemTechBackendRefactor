using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Quartz;
using RemTech.Bootstrap.Api.Configuration;
using RemTech.ContainedItems.Module.Grpc;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
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
RemTechApplicationSettings settings = RemTechApplicationSettings.ResolveByEnvironment(builder);
settings.UpDatabases();
builder.Services.InjectModules(settings);
builder.Services.ConfigureQuartz();
settings.ConfigureCors(builder);
builder.Services.AddGrpc();
builder.Services.AddOpenApi();
builder.Services.AddQuartzHostedService();
WebApplication app = builder.Build();
app.RegisterMiddlewares();
app.MapModulesEndpoints();
app.MapGrpcService<DuplicateIdsCheckService>();
app.Run();
