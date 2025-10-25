using Identity.Adapter.Jwt;
using Identity.Adapter.Notifier;
using Identity.Adapter.PasswordManager;
using Identity.Adapter.Storage.DependencyInjection;
using Identity.Domain.DependencyInjection;
using RemTech.Shared.Configuration.Options;
using Serilog;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Redis;

var builder = WebApplication.CreateBuilder(args);

Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

builder.Services.AddOptions<DatabaseOptions>().BindConfiguration(nameof(DatabaseOptions));
builder.Services.AddOptions<CacheOptions>().BindConfiguration(nameof(CacheOptions));
builder.Services.AddOptions<FrontendOptions>().BindConfiguration(nameof(FrontendOptions));

builder.Services.AddSingleton(logger);
builder.Services.InjectIdentityDomain();
builder.Services.InjectBcryptPasswordManager();
builder.Services.InjectIdentityStorageAdapter();
builder.Services.AddUsersNotifier();
builder.Services.AddPostgres();
builder.Services.AddRedis();

builder.Services.AddUsersNotifier();
builder.Services.AddIdentityJwt();
var app = builder.Build();

app.Run();

namespace Identity.WebApi
{
    public partial class Program { }
}
