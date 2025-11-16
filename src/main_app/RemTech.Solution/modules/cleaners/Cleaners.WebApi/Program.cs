using Cleaners.Adapter.Storage;
using Cleaners.Adapters.Cache;
using Cleaners.Domain;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.RabbitMq;
using Shared.Infrastructure.Module.Redis;
using Shared.Infrastructure.Module.Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<DatabaseOptions>().BindConfiguration(nameof(DatabaseOptions));
builder.Services.AddOptions<CacheOptions>().BindConfiguration(nameof(CacheOptions));
builder.Services.AddOptions<RabbitMqOptions>().BindConfiguration(nameof(RabbitMqOptions));

builder.Services.AddPostgres();
builder.Services.AddRedis();
builder.Services.AddRabbitMq();
builder.Services.AddSerilog();

builder.Services.AddCleanersDomain();

// builder.Services.AddCleanersOutboxProcessor();
// builder.Services.AddCleanerJob();
// builder.Services.AddQuartsHostedService();

builder.Services.AddCleanersStorageAdapter();
builder.Services.AddCachedCleaners();

var app = builder.Build();

app.Run();

namespace Cleaners.WebApi
{
    public partial class Program { }
}
