using System.Reflection;
using Identity.Adapter.Jwt;
using Identity.Adapter.Notifier;
using Identity.Adapter.PasswordManager;
using Identity.Adapter.Storage.DependencyInjection;
using Identity.Domain.DependencyInjection;
using Identity.Domain.Roles.ValueObjects;
using Identity.WebApi.Extensions;
using RemTech.Shared.Configuration.Options;
using Serilog;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Redis;

var builder = WebApplication.CreateBuilder(args);

Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

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

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

// await app.MigrateIdentityModule();
// await app.AddRoles(RoleName.Admin.Value, RoleName.User.Value, RoleName.Root.Value);
//
// if (app.Environment.IsDevelopment())
// {
//     await app.SeedUser("someuser@mail.com", "somelogin", "somePassword!23");
// }

app.Run();

namespace Identity.WebApi
{
    public partial class Program { }
}
