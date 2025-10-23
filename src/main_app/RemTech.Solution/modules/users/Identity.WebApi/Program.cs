using Identity.Adapter.Notifier;
using Identity.Adapter.PasswordManager;
using Identity.Adapter.Storage.DependencyInjection;
using Identity.Domain.DependencyInjection;
using RemTech.Shared.Configuration.Options;
using Serilog;
using Shared.Infrastructure.Module.Postgres;

var builder = WebApplication.CreateBuilder(args);

Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

builder.Services.AddSingleton(logger);
builder.Services.AddOptions<DatabaseOptions>().BindConfiguration(nameof(DatabaseOptions));
builder.Services.InjectIdentityDomain();
builder.Services.InjectBcryptPasswordManager();
builder.Services.InjectIdentityStorageAdapter();
builder.Services.AddUsersNotifier();
builder.Services.AddPostgres();

var app = builder.Build();
app.Run();

namespace Identity.WebApi
{
    public partial class Program { }
}
