using ParsedAdvertisements.Adapters.Messaging;
using ParsedAdvertisements.Adapters.Outbox;
using ParsedAdvertisements.Adapters.Storage;
using ParsedAdvertisements.Domain;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Quartz;
using Shared.Infrastructure.Module.RabbitMq;
using Shared.Infrastructure.Module.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<DatabaseOptions>().BindConfiguration(nameof(DatabaseOptions));
builder.Services.AddOptions<CacheOptions>().BindConfiguration(nameof(CacheOptions));
builder.Services.AddOptions<RabbitMqOptions>().BindConfiguration(nameof(RabbitMqOptions));

builder.Services.AddPostgres();
builder.Services.AddRabbitMq();
builder.Services.AddRedis();

builder.Services.AddParsedAdvertisementsDomain();
builder.Services.AddParsedAdvertisementsStorageAdapter();
builder.Services.AddParsedAdvertisementsOutboxAdapter();
builder.Services.AddParsedAdvertisementsMessagingAdapter();
builder.Services.ConfigureQuartzScheduler();

var app = builder.Build();

app.Run();