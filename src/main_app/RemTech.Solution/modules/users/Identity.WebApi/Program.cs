using Identity.Adapter.PasswordManager;
using Identity.Adapter.Storage.DependencyInjection;
using Identity.Domain.DependencyInjection;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<DatabaseOptions>().BindConfiguration(nameof(DatabaseOptions));
builder.Services.InjectIdentityDomain();
builder.Services.InjectBcryptPasswordManager();
builder.Services.InjectIdentityStorageAdapter();
builder.Services.AddPostgres();

var app = builder.Build();
app.Run();

namespace Identity.WebApi
{
    public partial class Program { }
}
