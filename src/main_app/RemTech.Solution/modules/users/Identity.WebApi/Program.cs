using System.Reflection;
using Identity.Domain.Roles.ValueObjects;
using Identity.WebApi;
using Identity.WebApi.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

builder.Services.AddPostgres();
builder.Services.AddRedis();
builder.Services.TryAddSingleton(logger);

builder.Services.AddIdentityModule();

var app = builder.Build();

await app.MigrateIdentityModule();
await app.SeedRoles(RoleName.User.Value, RoleName.Admin.Value, RoleName.Root.Value);
if (app.Environment.IsDevelopment())
{
    await app.SeedUser("testemail@mail.com", "testLogin", "superPassword!23");
    await app.SeedUser("admin@example.com", "admin", "AdminPass!2024");
    await app.SeedUser("user1@example.com", "user1", "UserPass!2024");
    await app.SeedUser("user2@example.com", "user2", "UserPass!2024");
    await app.SeedUser("test@test.com", "tester", "TestPass!2024");
    await app.SeedUser("dev@local.dev", "developer", "DevPass!2024");
    await app.SeedUser("admin@company.com", "admin", "AdminPass!2024");
    await app.SeedUser("superuser@company.com", "superuser", "SuperPass!2024");
    await app.SeedUser("manager1@company.com", "manager_a", "ManagerPass!2024");
    await app.SeedUser("manager2@company.com", "manager_b", "ManagerPass!2024");
    await app.SeedUser("alice@company.com", "alice", "UserPass!2024");
    await app.SeedUser("bob@company.com", "bob", "UserPass!2024");
    await app.SeedUser("charlie@company.com", "charlie", "UserPass!2024");
    await app.SeedUser("diana@company.com", "diana", "UserPass!2024");
    await app.SeedUser("evan@company.com", "evan", "UserPass!2024");
    await app.SeedUser("fiona@company.com", "fiona", "UserPass!2024");
    await app.SeedUser("alice@company.com", "alice", "UserPass!2024");
    await app.SeedUser("bob@company.com", "bob", "UserPass!2024");
    await app.SeedUser("charlie@company.com", "charlie", "UserPass!2024");
    await app.SeedUser("diana@company.com", "diana", "UserPass!2024");
    await app.SeedUser("evan@company.com", "evan", "UserPass!2024");
    await app.SeedUser("fiona@company.com", "fiona", "UserPass!2024");
    await app.SeedUser("verified@verified.com", "verified_user", "VerifiedPass!2024");
    await app.SeedUser("unverified@unverified.com", "unverified_user", "UnverifiedPass!2024");
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();

namespace Identity.WebApi
{
    public partial class Program { }
}
